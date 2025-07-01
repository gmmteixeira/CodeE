using Unity.Entities;
using UnityEngine;

public class CardPickupAuthoring : MonoBehaviour
{
    private class Baker : Baker<CardPickupAuthoring>
    {
        public override void Bake(CardPickupAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CardPickup
            {

            });
            AddComponent(entity, new Expiration { timeToLive = 30 });
        }
    }
}
public struct CardPickup : IComponentData
{

}