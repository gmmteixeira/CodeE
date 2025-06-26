using Unity.Entities;
using UnityEngine;

public class CardPowerupAuthoring : MonoBehaviour
{
    private class Baker : Baker<CardPowerupAuthoring>
    {
        public override void Bake(CardPowerupAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Expiration { timeToLive = 20 });
        }
    }
}
