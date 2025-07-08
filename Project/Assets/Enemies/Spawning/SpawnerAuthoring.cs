using Unity.Entities;
using UnityEngine;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject tankEnemyPrefab;
    public float cooldown = 0.5f;
    public int enemyCount;
    public float cooldownVarMin = 0.1f;
    public float cooldownVarMax = 0.3f;

    class SpawnerAuthoringBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnerProperties
            {
                enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                fastEnemyPrefab = GetEntity(authoring.fastEnemyPrefab, TransformUsageFlags.Dynamic),
                tankEnemyPrefab = GetEntity(authoring.tankEnemyPrefab, TransformUsageFlags.Dynamic),
                spawnedEnemy = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                scale = 1f,
                enemyCount = authoring.enemyCount,
                cooldown = authoring.cooldown,
                cooldownVarMin = authoring.cooldownVarMin,
                cooldownVarMax = authoring.cooldownVarMax,
                isSet = false,

            });
            AddComponent(entity, new Expiration
            {
                timeToLive = 5f
            });
        }
    }
}

public partial struct SpawnerProperties : IComponentData
{
    public Entity enemyPrefab;
    public Entity fastEnemyPrefab;
    public Entity tankEnemyPrefab;
    public Entity spawnedEnemy;
    public float scale;
    public int enemyCount;
    public float cooldown;
    public float cooldownVarMin;
    public float cooldownVarMax;
    public bool isSet;
}