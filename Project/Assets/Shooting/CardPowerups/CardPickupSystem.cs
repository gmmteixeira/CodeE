using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

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

            void ProcessCardHit(Entity card, Entity player)
            {
                if (!processedEnemies.Add(card))
                    return;

                ecb.DestroyEntity(card);
                CardPickup cardPickup = entityManager.GetComponentData<CardPickup>(card);
                Entity powerup = ecb.Instantiate(cardPickup.prefab);
                ecb.AddComponent(powerup, new CardPowerup
                {
                    cooldownIncrement = cardPickup.cooldownIncrement,
                    projectileCountIncrement = cardPickup.projectileCountIncrement,
                    spreadIncrement = cardPickup.spreadIncrement
                });

            }

            if (aIsCard && bIsPlayer)
            {
                ProcessCardHit(entityA, entityB);
            }
            else if (bIsCard && aIsPlayer)
            {
                ProcessCardHit(entityB, entityA);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        processedEnemies.Dispose();
    }
}
