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
        var updatedHealths = new NativeHashMap<Entity, int>(16, Allocator.Temp);

        foreach (var triggerEvent in sim.TriggerEvents)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            bool aIsProjectile = entityManager.HasComponent<ProjectileDamageProperties>(entityA);
            bool bIsProjectile = entityManager.HasComponent<ProjectileDamageProperties>(entityB);
            bool aHasHealth = entityManager.HasComponent<HealthProperties>(entityA);
            bool bHasHealth = entityManager.HasComponent<HealthProperties>(entityB);
            bool aIsArenaCollider = entityManager.HasComponent<ArenaCollider>(entityA);
            bool bIsArenaCollider = entityManager.HasComponent<ArenaCollider>(entityB);



            void ProcessProjectileHit(Entity projectile, Entity target)
            {
                if (!processedProjectiles.Add(projectile))
                    return;

                int currentHealth;
                if (!updatedHealths.TryGetValue(target, out currentHealth))
                {
                    currentHealth = entityManager.GetComponentData<HealthProperties>(target).health;
                }

                if (currentHealth > 0)
                {
                    var damage = entityManager.GetComponentData<ProjectileDamageProperties>(projectile);
                    var localTransform =  entityManager.GetComponentData<LocalTransform>(projectile);
                    currentHealth -= (int)damage.damage;
                    updatedHealths[target] = currentHealth;

                    var healthData = entityManager.GetComponentData<HealthProperties>(target);
                    healthData.health = currentHealth;
                    ecb.SetComponent(target, healthData);

                    if (currentHealth <= 0)
                    {
                        ecb.DestroyEntity(projectile);
                        if (damage.explosion > 1)
                        {
                            Entity explosion = ecb.Instantiate(damage.explosionPrefab);
                            ecb.AddComponent(explosion, new ExplosionProperties
                            {
                                damage = damage.damage
                            });
                            ecb.SetComponent(explosion, new LocalTransform
                            {
                                Position = localTransform.Position,
                                Rotation = localTransform.Rotation,
                                Scale = damage.explosion * 3
                            });
                        }
                        Entity hit = ecb.Instantiate(healthData.hitEffect);
                        ecb.AddComponent(hit, new Parent
                        {
                            Value = target
                        });
                    }
                }
            }

            void ProcessArenaCollision(Entity projectile)
            {
                var damage = entityManager.GetComponentData<ProjectileDamageProperties>(projectile);
                var localTransform =  entityManager.GetComponentData<LocalTransform>(projectile);
                
                if (!processedProjectiles.Add(projectile))
                    return;
                if (damage.explosion > 1)
                {
                    Entity explosion = ecb.Instantiate(damage.explosionPrefab);
                    ecb.AddComponent(explosion, new ExplosionProperties
                    {
                        damage = damage.damage
                    });
                    ecb.SetComponent(explosion, new LocalTransform
                    {
                        Position = localTransform.Position,
                        Rotation = localTransform.Rotation,
                        Scale = damage.explosion * 2
                    });
                }
                ecb.DestroyEntity(projectile);
            }

            if (aIsProjectile && bHasHealth)
            {
                ProcessProjectileHit(entityA, entityB);
            }
            else if (bIsProjectile && aHasHealth)
            {
                ProcessProjectileHit(entityB, entityA);
            }
            else if (aIsProjectile && bIsArenaCollider)
            {
                ProcessArenaCollision(entityA);
            }
            else if (bIsProjectile && aIsArenaCollider)
            {
                ProcessArenaCollision(entityB);
            }
        }

        processedProjectiles.Dispose();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        updatedHealths.Dispose();
    }
}