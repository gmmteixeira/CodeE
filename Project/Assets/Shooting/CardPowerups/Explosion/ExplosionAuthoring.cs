using Unity.Entities;
using UnityEngine;

public class ExplosionAuthoring : MonoBehaviour
{
    private class Baker : Baker<ExplosionAuthoring>
    {
        public override void Bake(ExplosionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Expiration { timeToLive = 2 });
            AddComponent(entity, new JustSpawnedExplosionTag());
        }
    }
}

public struct ExplosionProperties : IComponentData
{
    public float damage;
}
public struct JustSpawnedExplosionTag : IComponentData {}