using UnityEngine;
using Unity.Entities;

public class CardPjctlAuthoring : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    private class Baker : Baker<CardPjctlAuthoring>
    {
        public override void Bake(CardPjctlAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CardPjctlProperties { speed = authoring.speed });
            //AddComponent(entity, new CardPjctlProperties { lifeTime = authoring.lifeTime });
        }
    }
}

public struct CardPjctlProperties : IComponentData
{
    public float speed;
    //public float lifeTime;
}