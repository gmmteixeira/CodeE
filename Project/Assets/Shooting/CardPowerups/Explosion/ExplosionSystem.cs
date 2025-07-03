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

        foreach (var (explosion, transform, entity) in SystemAPI.Query<RefRO<ExplosionProperties>, RefRO<LocalTransform>>()
            .WithAll<JustSpawnedExplosionTag>()
            .WithEntityAccess())
        {
            var explosionPos = transform.ValueRO.Position;
            var explosionRadius = transform.ValueRO.Scale/2;
            var explosionDamage = explosion.ValueRO.damage;

            // Query all bodies in the world and check distance
            for (int i = 0; i < physicsWorld.NumBodies; i++)
            {
                var body = physicsWorld.Bodies[i];
                var bodyEntity = body.Entity;
                if (state.EntityManager.HasComponent<HomingBoidProperties>(bodyEntity) &&
                    state.EntityManager.HasComponent<HealthProperties>(bodyEntity))
                {
                    var bodyPos = physicsWorld.Bodies[i].WorldFromBody.pos;
                    if (math.distance(bodyPos, explosionPos) <= explosionRadius)
                    {
                        var health = state.EntityManager.GetComponentData<HealthProperties>(bodyEntity);
                        health.health -= (int)explosionDamage;
                        ecb.SetComponent(bodyEntity, health);
                    }
                }
            }

            ecb.RemoveComponent<JustSpawnedExplosionTag>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}