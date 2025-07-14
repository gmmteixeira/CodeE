using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
public partial class CardPickupSystem : SystemBase
{

    protected override void OnUpdate()
    {
        Vector3 playerPosition = Vector3.zero;
        if (SystemAPI.HasSingleton<PlayerSingletonData>())
        {
            // Get the player entity and its LocalTransform
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);
            playerPosition = playerTransform.Position;
        }
        else return;
        float shootingCooldown = 0;
        if (SystemAPI.HasSingleton<WeaponProperties>())
        {
            shootingCooldown = SystemAPI.GetSingleton<WeaponProperties>().cooldownTimer;
        }
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entities.WithAll<CardPickup>().ForEach((Entity entity, ref PhysicsMass mass, ref LocalTransform localTransform, ref CardPickup cardPickup) =>
        {
            mass.InverseInertia = float3.zero;
            if (cardPickup.lifetime > 0)
            {
                cardPickup.lifetime -= deltaTime;
                if (cardPickup.lifetime <= 0)
                {
                    Entity explosion = ecb.Instantiate(cardPickup.Explosion);
                    ecb.SetComponent(explosion, new LocalTransform
                    {
                        Position = localTransform.Position,
                        Rotation = localTransform.Rotation,
                        Scale = 10f
                    });
                    ecb.AddComponent(explosion, new ExplosionProperties
                    {
                        damage = 5,
                    });
                    ecb.DestroyEntity(entity);
                }
            }
            localTransform.Rotation = math.mul(localTransform.Rotation, quaternion.RotateY(math.radians(900 / (cardPickup.lifetime / 2) * deltaTime)));
            if (shootingCooldown <= 0)
            {
                float3 direction = math.normalize(new float3(playerPosition.x, 0, playerPosition.z) - new float3(localTransform.Position.x, 0, localTransform.Position.z));
                if (playerPosition.y < 3)
                {
                    localTransform.Position.xz += direction.xz * (5 + 30f / Mathf.Clamp(math.distance(localTransform.Position.xz, new float2(playerPosition.x, playerPosition.z)) / 2, 0.5f, 300f)) * deltaTime;
                }
                else
                {
                    localTransform.Position += math.normalize(new float3(playerPosition.x, playerPosition.y, playerPosition.z) - localTransform.Position) * 40 * deltaTime;
                }
            }
            if (localTransform.Position.y > 1.2) localTransform.Position.y -= 5 * deltaTime;
            else if (localTransform.Position.y < 1.2) localTransform.Position.y = 1.2f;
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct PowerupTriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Ensure all dependencies (including physics jobs) are complete before accessing simulation data
        state.Dependency.Complete();
        float3 playerPosition;
        if (SystemAPI.HasSingleton<PlayerSingletonData>())
        {
            playerPosition = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<PlayerSingletonData>()).Position;
        } else return;
        float3 effectSpawnPosition = new float3(playerPosition.x, playerPosition.y - 0.5f, playerPosition.z);
        var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        WeaponProperties weaponProperties;
        if (SystemAPI.HasSingleton<WeaponProperties>())
        {
            weaponProperties = SystemAPI.GetSingleton<WeaponProperties>();
        } else return;
        int score;
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            score = SystemAPI.GetSingleton<GameComponentData>().score;
        } else return;

        var sim = simSingleton.AsSimulation();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var processedEnemies = new NativeHashSet<Entity>(16, Allocator.Temp);

        foreach (var triggerEvent in sim.TriggerEvents)
        {

            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            bool aIsCard = SystemAPI.HasComponent<CardPickup>(entityA);
            bool bIsCard = SystemAPI.HasComponent<CardPickup>(entityB);
            bool aIsPlayer = SystemAPI.HasComponent<PlayerSingletonData>(entityA);
            bool bIsPlayer = SystemAPI.HasComponent<PlayerSingletonData>(entityB);

            void ProcessCardHit(Entity card)
            {
                if (!processedEnemies.Add(card))
                    return;

                Entity effectEntity = ecb.Instantiate(weaponProperties.topOffEffect);
                ecb.SetComponent(effectEntity, new LocalTransform
                {
                    Position = effectSpawnPosition,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                ecb.DestroyEntity(card);
                if ((weaponProperties.powerupLevel < 1 || weaponProperties.powerupDrain > 10f) && weaponProperties.powerupLevel < 3)
                {
                    weaponProperties.powerupLevel += 1;
                    weaponProperties.powerupDrain = 10f;

                }
                else if (weaponProperties.powerupLevel == 3 && weaponProperties.powerupDrain > 10f)
                {
                    weaponProperties.powerupDrain = 20f;
                    score += 5;
                }
                else
                {
                    weaponProperties.powerupDrain = 20f;
                }
            }

            if (aIsCard && bIsPlayer)
                {
                    ProcessCardHit(entityA);
                }
                else if (bIsCard && aIsPlayer)
                {
                    ProcessCardHit(entityB);
                }
        }
        
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            SystemAPI.SetSingleton(new GameComponentData { score = score });
        }
        if (SystemAPI.HasSingleton<WeaponProperties>())
        {
            SystemAPI.SetSingleton(weaponProperties);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        processedEnemies.Dispose();
    }
}
