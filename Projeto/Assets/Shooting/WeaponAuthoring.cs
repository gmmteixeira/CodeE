using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    public GameObject projectile;
    public float cooldownAmount;
    private float cooldownTimer;

    private class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new WeaponProperties
            {
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
                cooldownAmount = authoring.cooldownAmount,
                cooldownTimer = authoring.cooldownTimer
            });
        }
    }
}

public struct WeaponProperties : IComponentData
{
    public Entity projectile;
    public float cooldownAmount;
    public float cooldownTimer;
    
}