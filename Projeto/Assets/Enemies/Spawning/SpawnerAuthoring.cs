using Unity.Entities;
using UnityEngine;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
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
                enemyCount = authoring.enemyCount,
                cooldown = authoring.cooldown,
                cooldownVarMin = authoring.cooldownVarMin,
                cooldownVarMax = authoring.cooldownVarMax
            });
        }
    }
}

public partial struct SpawnerProperties : IComponentData
{
    public Entity enemyPrefab;
    public int enemyCount;
    public float cooldown;
    public float cooldownVarMin;
    public float cooldownVarMax;
}