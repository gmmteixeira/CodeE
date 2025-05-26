using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class ShootingAuthoring : MonoBehaviour
{

    public GameObject prefab;
    public int inacuracy;
    

    private class Baker : Baker<ShootingAuthoring>
    {
        public override void Bake(ShootingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootingProperties
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                inacuracy = authoring.inacuracy,

            });
        }
    }
}

public struct ShootingProperties : IComponentData
{
    public Entity prefab;
    public int inacuracy;
}