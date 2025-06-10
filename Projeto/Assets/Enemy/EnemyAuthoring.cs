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
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new EnemyProperties
            {
                forwardSpeed = authoring.forwardSpeed,
                turningSpeed = authoring.turningSpeed,
                health = authoring.health
            });
        }
    }
}

public struct EnemyProperties : IComponentData
{
    public float forwardSpeed;
    public float turningSpeed;
    public int health;
}