using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;

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

        foreach (var triggerEvent in sim.TriggerEvents)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            bool aIsProjectile = state.EntityManager.HasComponent<ProjectileDamageProperties>(entityA);
            bool bIsProjectile = state.EntityManager.HasComponent<ProjectileDamageProperties>(entityB);
            bool aHasHealth = state.EntityManager.HasComponent<HealthProperties>(entityA);
            bool bHasHealth = state.EntityManager.HasComponent<HealthProperties>(entityB);

            // Projectile hits health entity
            if (aIsProjectile && bHasHealth)
            {
                var damage = state.EntityManager.GetComponentData<ProjectileDamageProperties>(entityA).damage;
                var health = state.EntityManager.GetComponentData<HealthProperties>(entityB);
                health.health -= (int)damage;
                ecb.SetComponent(entityB, health);
                ecb.DestroyEntity(entityA);
            }
            else if (bIsProjectile && aHasHealth)
            {
                var damage = state.EntityManager.GetComponentData<ProjectileDamageProperties>(entityB).damage;
                var health = state.EntityManager.GetComponentData<HealthProperties>(entityA);
                health.health -= (int)damage;
                ecb.SetComponent(entityA, health);
                ecb.DestroyEntity(entityB);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}