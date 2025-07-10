using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial class EnemySystem : SystemBase
{
    protected override void OnCreate()
    {

    }
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Vector3 playerPosition = Vector3.zero;
        if (SystemAPI.HasSingleton<PlayerSingletonData>())
        {
            // Get the player entity and its LocalTransform
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);
            playerPosition = playerTransform.Position;
        } else return;
        
        int score = 0;
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            score = SystemAPI.GetSingleton<GameComponentData>().score;
        } else return;
        
        Entities
        .WithAll<PhysicsVelocity>()
        .ForEach((ref PhysicsMass mass, ref PhysicsVelocity velocity, ref LocalTransform localTransform, in HomingBoidProperties homingBoidProperties) =>
        {
            mass.InverseInertia = float3.zero;
            float3 forward = math.forward(localTransform.Rotation);
            velocity.Linear = forward * homingBoidProperties.forwardSpeed;

            float3 lTP = localTransform.Position;
            localTransform.Rotation = Quaternion.Slerp(
                localTransform.Rotation,
                Quaternion.LookRotation((playerPosition - new Vector3(lTP.x, lTP.y, lTP.z)).normalized),
                deltaTime * homingBoidProperties.turningSpeed);

        }).ScheduleParallel();


        Entities.WithoutBurst().ForEach((Entity entity, in HealthProperties damageProperties) =>
        {
            if (damageProperties.health <= 0)
            {
                Entity deathEntity = ecb.Instantiate(damageProperties.deathEffect);
                ecb.SetComponent(deathEntity, new LocalTransform
                {
                    Position = SystemAPI.GetComponent<LocalTransform>(entity).Position,
                    Scale = 1
                });

                if (UnityEngine.Random.Range(0, 100) < damageProperties.dropChance)
                {
                    Entity cardEntity = ecb.Instantiate(damageProperties.cardPickup);
                    ecb.SetComponent(cardEntity, new LocalTransform
                    {
                        Position = SystemAPI.GetComponent<LocalTransform>(entity).Position,
                        Rotation = quaternion.identity,
                        Scale = 100
                    });
                    
                }
                
                ecb.DestroyEntity(entity);
                score += damageProperties.scoreReward;
                
            }

        }).Run();
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            SystemAPI.SetSingleton(new GameComponentData { score = score });
        }

        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct EnemyTriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        var sim = simSingleton.AsSimulation();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var entityManager = state.EntityManager;

        var processedEnemies = new NativeHashSet<Entity>(16, Allocator.Temp);

        foreach (var triggerEvent in sim.TriggerEvents)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            bool aIsEnemy = SystemAPI.HasComponent<HomingBoidProperties>(entityA);
            bool bIsEnemy = SystemAPI.HasComponent<HomingBoidProperties>(entityB);
            bool aIsPlayer = SystemAPI.HasComponent<PlayerSingletonData>(entityA);
            bool bIsPlayer = SystemAPI.HasComponent<PlayerSingletonData>(entityB);
            bool aIsLaser = SystemAPI.HasComponent<LaserProperties>(entityA);
            bool bIsLaser = SystemAPI.HasComponent<LaserProperties>(entityB);

            void ProcessEnemyHit(Entity enemy, Entity player)
            {
                if (!processedEnemies.Add(enemy))
                    return;

                if (entityManager.GetComponentData<PlayerSingletonData>(player).isAlive)
                {
                    ecb.SetComponent(player, new PlayerSingletonData { isAlive = false });
                }
            }
            void ProcessLaserHit(Entity laser, Entity target)
            {
                if (!processedEnemies.Add(laser))
                    return;

                
                if (entityManager.HasComponent<HealthProperties>(target) && entityManager.GetComponentData<LaserProperties>(laser).activeTime > 0)
                {
                    var health = entityManager.GetComponentData<HealthProperties>(target);
                    health.health -= entityManager.GetComponentData<LaserProperties>(laser).damage;
                    ecb.SetComponent(target, health);
                }
            }

            if (aIsEnemy && bIsPlayer)
            {
                ProcessEnemyHit(entityA, entityB);
            }
            else if (bIsEnemy && aIsPlayer)
            {
                ProcessEnemyHit(entityB, entityA);
            }
            else if (aIsLaser && bIsEnemy)
            {
                ProcessLaserHit(entityA, entityB);
            }
            else if (bIsLaser && aIsEnemy)
            {
                ProcessLaserHit(entityB, entityA);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        processedEnemies.Dispose();
    }
}
