using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public partial class ProjectileSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref LocalTransform localTransform, in ProjectileFlightProperties projectileProperties) =>
        {
            if (math.distance(localTransform.Position, new float3(0, 0, 0)) > projectileProperties.distanceLimit) { ecbParallel.DestroyEntity(entityInQueryIndex, entity); }

        }).ScheduleParallel();

        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ProjectileTriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        
        var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        var sim = simSingleton.AsSimulation();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        state.Dependency.Complete();

        var entityManager = state.EntityManager;

        var processedProjectiles = new NativeHashSet<Entity>(16, Allocator.Temp);

        foreach (var triggerEvent in sim.TriggerEvents)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            bool aIsProjectile = entityManager.HasComponent<ProjectileDamageProperties>(entityA);
            bool bIsProjectile = entityManager.HasComponent<ProjectileDamageProperties>(entityB);
            bool aHasHealth = entityManager.HasComponent<HealthProperties>(entityA);
            bool bHasHealth = entityManager.HasComponent<HealthProperties>(entityB);

            // Helper function to process projectile hit
            void ProcessProjectileHit(Entity projectile, Entity target)
            {
                if (!processedProjectiles.Add(projectile))
                    return;

                var damage = entityManager.GetComponentData<ProjectileDamageProperties>(projectile).damage;
                var health = entityManager.GetComponentData<HealthProperties>(target);
                health.health -= (int)damage;
                ecb.SetComponent(target, health);
                ecb.DestroyEntity(projectile);
                Entity hit = ecb.Instantiate(health.hitEffect);
                ecb.AddComponent(hit, new Parent
                {
                    Value = target
                });
            }

            if (aIsProjectile && bHasHealth)
            {
                ProcessProjectileHit(entityA, entityB);
            }
            else if (bIsProjectile && aHasHealth)
            {
                ProcessProjectileHit(entityB, entityA);
            }
        }

        processedProjectiles.Dispose();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}