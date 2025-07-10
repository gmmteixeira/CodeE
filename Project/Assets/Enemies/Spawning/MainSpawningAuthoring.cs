using Unity.Entities;
using UnityEngine;

public class MainSpawningAuthoring : MonoBehaviour
{
    public GameObject swarmerEnemySpawner;
    public GameObject fastEnemySpawner;
    public GameObject tankEnemySpawner;
    public float cooldown = 5f;
    public float cooldownVar = 1f;
    public int ringdistance;
    public float yOffset = 0f;
    class Baker : Baker<MainSpawningAuthoring>
    {
        public override void Bake(MainSpawningAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MainSpawningProperties
            {
                swarmerEnemySpawner = GetEntity(authoring.swarmerEnemySpawner, TransformUsageFlags.Dynamic),
                fastEnemySpawner = GetEntity(authoring.fastEnemySpawner, TransformUsageFlags.Dynamic),
                tankEnemySpawner = GetEntity(authoring.tankEnemySpawner, TransformUsageFlags.Dynamic),
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
    public Entity swarmerEnemySpawner;
    public Entity fastEnemySpawner;
    public Entity tankEnemySpawner;
    public float cooldown;
    public float cooldownVar;
    public float ringdistance;
    public float yOffset;
}