using Unity.Entities;
using UnityEngine;

public class CardPickupAuthoring : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float lifetime;
    private class Baker : Baker<CardPickupAuthoring>
    {
        public override void Bake(CardPickupAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CardPickup
            {
                Explosion = GetEntity(authoring.explosionPrefab, TransformUsageFlags.Dynamic),
                lifetime = authoring.lifetime
            });
        }
    }
}
public struct CardPickup : IComponentData
{
    public Entity Explosion;
    public float lifetime;
}