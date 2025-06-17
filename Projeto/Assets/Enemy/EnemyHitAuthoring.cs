using Unity.Entities;
using UnityEngine;

public class EnemyHitAuthoring : MonoBehaviour
{
    public float timeToLive;

    private class Baker : Baker<EnemyHitAuthoring>
    {
        public override void Bake(EnemyHitAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Expiration
            {
                timeToLive = authoring.timeToLive
            });
        }
    }
}