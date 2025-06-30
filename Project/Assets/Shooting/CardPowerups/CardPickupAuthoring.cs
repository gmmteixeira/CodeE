using Unity.Entities;
using UnityEngine;

public class CardPickupAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float cooldownModifier;
    public float projectileCountModifier;
    public float spreadModifier;
    public float explosionModifier;
    private class Baker : Baker<CardPickupAuthoring>
    {
        public override void Bake(CardPickupAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CardPickup
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                cooldownModifier = authoring.cooldownModifier,
                projectileCountModifier = authoring.projectileCountModifier,
                spreadModifier = authoring.spreadModifier,
                explosionModifier = authoring.explosionModifier
            });
            AddComponent(entity, new Expiration { timeToLive = 30 });
        }
    }
}
public struct CardPickup : IComponentData
{
    public Entity prefab;
    public float cooldownModifier;
    public float projectileCountModifier;
    public float spreadModifier;
    public float explosionModifier;
}