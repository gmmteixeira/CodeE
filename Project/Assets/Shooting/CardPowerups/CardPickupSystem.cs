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
        } else return;
        float shootingCooldown = 0;
        if (SystemAPI.HasSingleton<WeaponProperties>())
        {
            shootingCooldown = SystemAPI.GetSingleton<WeaponProperties>().cooldownTimer;
        }
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entities.ForEach((ref PhysicsMass mass, ref LocalTransform localTransform, in CardPickup cardPickup) =>
        {
            mass.InverseInertia = float3.zero;
            localTransform.Rotation = math.mul(localTransform.Rotation, quaternion.RotateY(math.radians(180 * deltaTime)));
            if (shootingCooldown <= 0)
            {
                float3 direction = math.normalize(new float3(playerPosition.x, 0, playerPosition.z) - new float3(localTransform.Position.x, 0, localTransform.Position.z));
                localTransform.Position.xz += direction.xz * 10f * deltaTime;
            }
            if (localTransform.Position.y > 1) localTransform.Position.y -= 5 * deltaTime;
            else if (localTransform.Position.y < 1) localTransform.Position.y = 1;
        })
        .ScheduleParallel();
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

        var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        WeaponProperties weaponProperties;
        if (SystemAPI.HasSingleton<WeaponProperties>())
        {
            weaponProperties = SystemAPI.GetSingleton<WeaponProperties>();
        } else return;
        int score = 0;
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            score = SystemAPI.GetSingleton<GameComponentData>().score;
        } else return;

        var sim = simSingleton.AsSimulation();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var entityManager = state.EntityManager;

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

                ecb.DestroyEntity(card);
                if ((weaponProperties.powerupLevel < 1 || weaponProperties.powerupDrain > 10f) && weaponProperties.powerupLevel < 3)
                {
                    weaponProperties.powerupLevel += 1;
                    weaponProperties.powerupDrain = 10f;
                }
                else if (weaponProperties.powerupLevel == 3 && weaponProperties.powerupDrain > 10f)
                {
                    weaponProperties.powerupDrain = 20f;
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
