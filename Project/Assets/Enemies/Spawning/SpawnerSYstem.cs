using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        PlayerSingletonData playerData = default;
        GameComponentData gameData = default;
        if (SystemAPI.HasSingleton<PlayerSingletonData>())
        {
            playerData = SystemAPI.GetSingleton<PlayerSingletonData>();
        }
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            gameData = SystemAPI.GetSingleton<GameComponentData>();
        }
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities.WithAll<SpawnerProperties>().ForEach((Entity entity, ref SpawnerProperties spawner, ref LocalTransform localTransform) =>
        {
            spawner.cooldown -= deltaTime;
            if (!spawner.isSet)
            {
                spawner.isSet = true;
                float randomFloat = UnityEngine.Random.Range(0f, 1f);

                if (randomFloat < 0.25f && gameData.score >= 200)
                {
                    spawner.spawnedEnemy = spawner.fastEnemyPrefab;
                    spawner.scale = 80f;
                    spawner.enemyCount = 3;
                }
                else if (randomFloat < 0.1f && gameData.score >= 400)
                {
                    spawner.spawnedEnemy = spawner.tankEnemyPrefab;
                    spawner.scale = 80f;
                    spawner.enemyCount = 1;
                }
                else
                {
                    spawner.spawnedEnemy = spawner.enemyPrefab;
                    spawner.scale = 0.55f;
                    spawner.enemyCount = 10;
                }
            }
            
            if (spawner.cooldown <= 0f && playerData.isAlive)
            {
                spawner.cooldown = spawner.cooldownVarMin + UnityEngine.Random.Range(0f, spawner.cooldownVarMax);

                if (spawner.enemyCount > 0)
                {
                    spawner.enemyCount--;
                    Entity enemyEntity = ecb.Instantiate(spawner.spawnedEnemy);
                    float3 direction = math.normalize(new float3(0f, 0f, 0f) - localTransform.Position);
                    float3 toCenter = math.normalize(new float3(0f, 0f, 0f) - localTransform.Position);
                    float yaw = math.atan2(toCenter.x, toCenter.z);
                    quaternion rotation = quaternion.Euler(math.radians(-90f), yaw, 0f);
                    ecb.SetComponent(enemyEntity, new LocalTransform
                    {
                        Position = localTransform.Position + new float3(
                            UnityEngine.Random.Range(-5f, 5f),
                            0f,
                            UnityEngine.Random.Range(-5f, 5f)),
                        Rotation = rotation,
                        Scale = spawner.scale
                    });
                }

            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
