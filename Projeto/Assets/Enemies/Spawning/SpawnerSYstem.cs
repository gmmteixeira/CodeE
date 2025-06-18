using Unity.Entities;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpawnerProperties spawner) =>
        {
            // Update the spawner's cooldown
            spawner.cooldown -= Time.DeltaTime;
            if (spawner.cooldown <= 0f)
            {
                spawner.cooldown = spawner.cooldownVarMin + Random.Range(0f, spawner.cooldownVarMax);
                
                Entity enemyEntity = EntityManager.Instantiate(spawner.enemyPrefab);
                EntityManager.SetComponentData(enemyEntity, new Translation
                {
                    Value = new float3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f))
                });
            }
        }).Schedule();
    }
}
