using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class EnemyAuthoring : MonoBehaviour
{
    public float forwardSpeed;
    public float turningSpeed;
    public int health;
    public GameObject deathEffect;

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
                deathEffect = GetEntity(authoring.deathEffect, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct HomingBoidProperties : IComponentData
{
    public float forwardSpeed;
    public float turningSpeed;
}

public struct HealthProperties : IComponentData {public int health; public Entity deathEffect;}