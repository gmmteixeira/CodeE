using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial class ExplosionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entities.ForEach((ref PhysicsMass mass, ref LocalTransform localTransform, in ExplosionProperties explosionProperties) =>
        {
            mass.InverseInertia = float3.zero;
            localTransform.Scale -= 20 * deltaTime;
            if (localTransform.Scale < 0)
            {
                localTransform.Scale = 0;
            }
        }).ScheduleParallel();
    }
}

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct ExplosionDamageOnSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var damageMap = new NativeHashMap<Entity, int>(physicsWorld.NumBodies, Allocator.Temp);

        foreach (var (explosion, transform, entity) in SystemAPI.Query<RefRO<ExplosionProperties>, RefRO<LocalTransform>>()
                     .WithAll<JustSpawnedExplosionTag>()
                     .WithEntityAccess())
        {
            var explosionPos = transform.ValueRO.Position;
            var explosionRadius = transform.ValueRO.Scale / 2;
            var explosionDamage = (int)explosion.ValueRO.damage;

            for (int i = 0; i < physicsWorld.NumBodies; i++)
            {
                var body = physicsWorld.Bodies[i];
                var bodyEntity = body.Entity;

                if (state.EntityManager.HasComponent<HomingBoidProperties>(bodyEntity) &&
                    state.EntityManager.HasComponent<HealthProperties>(bodyEntity))
                {
                    var bodyPos = body.WorldFromBody.pos;

                    if (math.distance(bodyPos, explosionPos) <= explosionRadius)
                    {
                        // Accumulate damage
                        if (damageMap.ContainsKey(bodyEntity))
                            damageMap[bodyEntity] += explosionDamage;
                        else
                            damageMap[bodyEntity] = explosionDamage;
                    }
                }
            }

            ecb.RemoveComponent<JustSpawnedExplosionTag>(entity);
        }

        // Apply accumulated damage
        foreach (var kvp in damageMap)
        {
            var entity = kvp.Key;
            var damage = kvp.Value;

            var health = state.EntityManager.GetComponentData<HealthProperties>(entity);
            health.health -= damage;
            ecb.SetComponent(entity, health);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        damageMap.Dispose();
    }
}
