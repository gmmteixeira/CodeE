using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class EnemyAuthoring : MonoBehaviour
{
    public float forwardSpeed;
    public float turningSpeed;
    public int health;

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
            AddComponent(entity, new DamageProperties
            {
                health = authoring.health
            });
        }
    }
}

public struct HomingBoidProperties : IComponentData
{
    public float forwardSpeed;
    public float turningSpeed;
}

public struct DamageProperties : IComponentData {public int health; }