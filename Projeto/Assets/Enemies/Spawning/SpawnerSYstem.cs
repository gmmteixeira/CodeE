using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities.WithAll<SpawnerProperties>().ForEach((Entity entity, ref SpawnerProperties spawner, ref LocalTransform localTransform) =>
        {

            spawner.cooldown -= deltaTime;
            
            if (spawner.cooldown <= 0f)
            {
                spawner.cooldown = spawner.cooldownVarMin + UnityEngine.Random.Range(0f, spawner.cooldownVarMax);

                if (spawner.enemyCount > 0)
                {
                    spawner.enemyCount--;
                    Entity enemyEntity = ecb.Instantiate(spawner.enemyPrefab);
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
                        Scale = .55f
                    });
                }

            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
