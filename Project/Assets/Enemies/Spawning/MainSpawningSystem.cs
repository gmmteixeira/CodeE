using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class MainSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        if (!SystemAPI.HasSingleton<MainSpawningProperties>() || !SystemAPI.HasSingleton<GameComponentData>())
            return;

        ref var mainSpawning = ref SystemAPI.GetSingletonRW<MainSpawningProperties>().ValueRW;
        ref var score = ref SystemAPI.GetSingletonRW<GameComponentData>().ValueRW.score;

        mainSpawning.cooldown -= deltaTime;
        if (mainSpawning.cooldown <= 0f)
        {
            // BALANCING: Score based progression curve
            try
            {
                mainSpawning.cooldown = 5 / (score / 30) + 0.5f + UnityEngine.Random.Range(0f, mainSpawning.cooldownVar);
            }
            catch (System.DivideByZeroException)
            {
                mainSpawning.cooldown = 10;
            }

            float angle = UnityEngine.Random.Range(0f, 2 * math.PI);
            float randomChance = UnityEngine.Random.Range(0f, 1f);
            Entity spawner = mainSpawning.swarmerEnemySpawner;
            if (score >= 400 && randomChance < 0.1f)
            {
                spawner = mainSpawning.tankEnemySpawner;
            }
            else if (score >= 200 && randomChance < 0.3f)
            {
                spawner = mainSpawning.fastEnemySpawner;
            }
            Entity spawnedSpawner = ecb.Instantiate(spawner);
            ecb.SetComponent(spawnedSpawner, new LocalTransform
            {
                Position = new float3(math.cos(angle) * mainSpawning.ringdistance,
                                      mainSpawning.yOffset,
                                      math.sin(angle) * mainSpawning.ringdistance),
                Rotation = Quaternion.identity,
                Scale = 1f
            });
        }
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

}
