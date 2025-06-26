using Unity.Entities;
using UnityEngine;

public class CardPickupAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float cooldownIncrement;
    public int projectileCountIncrement;
    public float spreadIncrement;
    private class Baker : Baker<CardPickupAuthoring>
    {
        public override void Bake(CardPickupAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CardPickup
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                cooldownIncrement = authoring.cooldownIncrement,
                projectileCountIncrement = authoring.projectileCountIncrement,
                spreadIncrement = authoring.spreadIncrement
            });
            AddComponent(entity, new Expiration { timeToLive = 30 });
        }
    }
}
public struct CardPickup : IComponentData
{
    public Entity prefab;
    public float cooldownIncrement;
    public int projectileCountIncrement;
    public float spreadIncrement;
}