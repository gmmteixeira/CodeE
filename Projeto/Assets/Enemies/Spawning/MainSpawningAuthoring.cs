using Unity.Entities;
using UnityEngine;

class MainSpawningAuthoring : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public float cooldown = 5f;
    public float cooldownVar = 1f;
    public int ringdistance;
    public float yOffset = 0f;
    class MainSpawningAuthoringBaker : Baker<MainSpawningAuthoring>
    {
        public override void Bake(MainSpawningAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MainSpawningProperties
            {
                spawnerPrefab = GetEntity(authoring.spawnerPrefab, TransformUsageFlags.Dynamic),
                cooldown = authoring.cooldown,
                cooldownVar = authoring.cooldownVar,
                ringdistance = authoring.ringdistance,
                yOffset = authoring.yOffset
            });
        }
    } 
}

public partial struct MainSpawningProperties : IComponentData
{
    public Entity spawnerPrefab;
    public float cooldown;
    public float cooldownVar;
    public float ringdistance;
    public float yOffset;
}