using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class LaserAuthoring : MonoBehaviour
{
    private class Baker : Baker<LaserAuthoring>
    {
        public override void Bake(LaserAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Expiration { timeToLive = 1.5f });
        }
    }
}

public struct LaserProperties : IComponentData
{
    public float activeTime;
    public int damage;
}