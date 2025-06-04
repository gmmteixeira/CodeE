using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class WeaponAuthoring : MonoBehaviour
{
    public GameObject projectile;

    private class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new WeaponProperties
            {
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct WeaponProperties : IComponentData
{
    public Entity projectile;
    
}

