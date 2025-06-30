using Unity.Entities;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    public GameObject projectile;
    public float cooldownAmount;
    private float cooldownTimer;
    public int projectileCount;
    public float spread;
    public float damage;
    public float speed;
    public GameObject soundEffect;
    public GameObject explosionPrefab;

    private class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new WeaponProperties
            {
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
                cooldownAmount = authoring.cooldownAmount,
                cooldownTimer = authoring.cooldownTimer,
                speed = authoring.speed,
                projectileCount = authoring.projectileCount,
                spread = authoring.spread,
                damage = authoring.damage,
                soundEffect = GetEntity(authoring.soundEffect, TransformUsageFlags.Dynamic),
                explosionPrefab = GetEntity(authoring.explosionPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct WeaponProperties : IComponentData
{
    public Entity projectile;
    public float cooldownAmount;
    public float cooldownTimer;
    public int projectileCount;
    public float spread;
    public float damage;
    public float speed;
    public Entity soundEffect;
    public Entity explosionPrefab;
}