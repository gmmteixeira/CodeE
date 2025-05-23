using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial struct CardPjctlSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<CardPjctlProperties> cardPjctlSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<CardPjctlProperties>>())
        {
            localTransform.ValueRW.Position += new float3(0, 0, cardPjctlSpeed.ValueRO.speed * SystemAPI.Time.DeltaTime);
            
        }
    }
}
