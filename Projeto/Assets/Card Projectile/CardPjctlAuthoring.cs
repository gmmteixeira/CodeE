using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class CardPjctlAuthoring : MonoBehaviour
{
    public float speed = 10f;
    //public GameObject arenaObj;
    //public int radius;
    

    private class Baker : Baker<CardPjctlAuthoring>
    {
        public override void Bake(CardPjctlAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CardPjctlProperties
            {
                speed = authoring.speed,
                //radius = authoring.radius,
                //arenaPos = authoring.arenaObj.transform.position,

            });
        }
    }
}

public struct CardPjctlProperties : IComponentData
{
    public float speed;
    //public float3 arenaPos;
    //public int radius;
}