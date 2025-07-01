using Unity.Entities;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    [Header("General")]
    public GameObject projectile;
    public float speed;
    public int powerupLevel;
    public GameObject soundEffect;
    public GameObject explosionPrefab;

    [Header("No Powerup")]
    public float lvl0Cooldown;
    public int lvl0ProjectileCount;
    public float lvl0Spread;
    public int lvl0Damage;
    public float lvl0Explosion;
    
    [Header("Powerup Level 1")]
    public float lvl1Cooldown;
    public int lvl1ProjectileCount;
    public float lvl1Spread;
    public int lvl1Damage;
    public float lvl1Explosion;
    
    [Header("Powerup Level 2")]
    public float lvl2Cooldown;
    public int lvl2ProjectileCount;
    public float lvl2Spread;
    public int lvl2Damage;
    public float lvl2Explosion;
    
    [Header("Powerup Level 3")]
    public float lvl3Cooldown;
    public int lvl3ProjectileCount;
    public float lvl3Spread;
    public int lvl3Damage;
    public float lvl3Explosion;

    private float cooldownTimer;

    private class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new WeaponProperties
            {
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
                lvl0Cooldown = authoring.lvl0Cooldown,
                lvl0ProjectileCount = authoring.lvl0ProjectileCount,
                lvl0Spread = authoring.lvl0Spread,
                lvl0Damage = authoring.lvl0Damage,
                lvl0Explosion = authoring.lvl0Explosion,

                lvl1Cooldown = authoring.lvl1Cooldown,
                lvl1ProjectileCount = authoring.lvl1ProjectileCount,
                lvl1Spread = authoring.lvl1Spread,
                lvl1Damage = authoring.lvl1Damage,
                lvl1Explosion = authoring.lvl1Explosion,

                lvl2Cooldown = authoring.lvl2Cooldown,
                lvl2ProjectileCount = authoring.lvl2ProjectileCount,
                lvl2Spread = authoring.lvl2Spread,
                lvl2Damage = authoring.lvl2Damage,
                lvl2Explosion = authoring.lvl2Explosion,

                lvl3Cooldown = authoring.lvl3Cooldown,
                lvl3ProjectileCount = authoring.lvl3ProjectileCount,
                lvl3Spread = authoring.lvl3Spread,
                lvl3Damage = authoring.lvl3Damage,
                lvl3Explosion = authoring.lvl3Explosion,

                powerupLevel = authoring.powerupLevel,
                cooldownTimer = 0f,
                speed = authoring.speed,
                soundEffect = GetEntity(authoring.soundEffect, TransformUsageFlags.Dynamic),
                explosionPrefab = GetEntity(authoring.explosionPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct WeaponProperties : IComponentData
{
    public Entity projectile;
    public int powerupLevel;

    // No powerup
    public float lvl0Cooldown;
    public int lvl0ProjectileCount;
    public float lvl0Spread;
    public int lvl0Damage;
    public float lvl0Explosion;
    
    // Powerup level 1
    public float lvl1Cooldown;
    public int lvl1ProjectileCount;
    public float lvl1Spread;
    public int lvl1Damage;
    public float lvl1Explosion;
    
    // Powerup level 2
    public float lvl2Cooldown;
    public int lvl2ProjectileCount;
    public float lvl2Spread;
    public int lvl2Damage;
    public float lvl2Explosion;
    
    // Powerup level 3
    public float lvl3Cooldown;
    public int lvl3ProjectileCount;
    public float lvl3Spread;
    public int lvl3Damage;
    public float lvl3Explosion;

    public float cooldownTimer;
    public float speed;
    public Entity soundEffect;
    public Entity explosionPrefab;
}