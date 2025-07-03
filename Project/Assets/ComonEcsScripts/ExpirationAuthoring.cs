using Unity.Entities;
using UnityEngine;

public class ExpirationAuthoring : MonoBehaviour
{
    public float timeToLive;

    private class Baker : Baker<ExpirationAuthoring>
    {
        public override void Bake(ExpirationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Expiration
            {
                timeToLive = authoring.timeToLive
            });
        }
    }
}