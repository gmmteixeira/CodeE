using Unity.Entities;
using UnityEngine;

public class EnemyDeathAuthoring : MonoBehaviour
{
    public float timeToLive;

    private class Baker : Baker<EnemyDeathAuthoring>
    {
        public override void Bake(EnemyDeathAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Expiration
            {
                timeToLive = authoring.timeToLive
            });
        }
    }
}