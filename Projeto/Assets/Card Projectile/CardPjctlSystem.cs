using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
public partial struct CardPjctlSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRW<LocalTransform> localTransform, RefRW<CardPjctlProperties> cardPjctlProperties, Entity entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<CardPjctlProperties>>().WithEntityAccess())
        {

            localTransform.ValueRW.Position += new float3(0, 0, cardPjctlProperties.ValueRO.speed * deltaTime);

            // if (math.distance(localTransform.ValueRO.Position, cardPjctlProperties.ValueRO.arenaPos) > cardPjctlProperties.ValueRO.radius)
            // {
            //     ecb.DestroyEntity(entity);
            // }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
