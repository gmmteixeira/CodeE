using Unity.Entities;
using UnityEngine;

class MainSpawningAuthoring : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public float cooldown = 5f; // Cooldown in seconds

    class MainSpawningAuthoringBaker : Baker<MainSpawningAuthoring>
    {
        public override void Bake(MainSpawningAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MainSpawningProperties
            {
                spawnerPrefab = GetEntity(authoring.spawnerPrefab, TransformUsageFlags.Dynamic),
                cooldown = authoring.cooldown
            });
        }
    } 
}

public partial struct MainSpawningProperties : IComponentData
{
    public Entity spawnerPrefab;
    public float cooldown;
}