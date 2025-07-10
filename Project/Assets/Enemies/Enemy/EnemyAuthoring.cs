using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float forwardSpeed;
    public float turningSpeed;
    public int health;
    public int scoreReward;
    public GameObject deathEffect;
    public GameObject hitEffect;
    public GameObject cardPickup;
    public int dropChance;

    private class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HomingBoidProperties
            {
                forwardSpeed = authoring.forwardSpeed,
                turningSpeed = authoring.turningSpeed,
            });
            AddComponent(entity, new HealthProperties
            {
                health = authoring.health,
                deathEffect = GetEntity(authoring.deathEffect, TransformUsageFlags.Dynamic),
                hitEffect = GetEntity(authoring.hitEffect, TransformUsageFlags.Dynamic),
                cardPickup = GetEntity(authoring.cardPickup, TransformUsageFlags.Dynamic),
                dropChance = authoring.dropChance,
                scoreReward = authoring.scoreReward
            });
        }
    }
}

public struct HomingBoidProperties : IComponentData
{
    public float forwardSpeed;
    public float turningSpeed;
}

public struct HealthProperties : IComponentData
{
    public int health;
    public Entity deathEffect;
    public Entity hitEffect;
    public Entity cardPickup;
    public int dropChance;
    public int scoreReward;
}