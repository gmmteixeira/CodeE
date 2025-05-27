using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
public partial struct CardPjctlSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRW<LocalTransform> localTransform, RefRW<CardPjctlProperties> cardPjctlProperties, Entity entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<CardPjctlProperties>>().WithEntityAccess())
        {

            float speed = cardPjctlProperties.ValueRO.speed * deltaTime;
            float3 forward = math.forward(localTransform.ValueRO.Rotation);
            localTransform.ValueRW.Position += forward * speed;

            // if (math.distance(localTransform.ValueRO.Position, cardPjctlProperties.ValueRO.arenaPos) > cardPjctlProperties.ValueRO.radius)
            // {
            //     ecb.DestroyEntity(entity);
            // }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
