using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;

public partial struct ProjectileSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileProperties>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float deltaTime = SystemAPI.Time.DeltaTime;

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach ((RefRW<LocalTransform> localTransform, RefRO<ProjectileProperties> projectileProperties, Entity entity)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ProjectileProperties>>().WithEntityAccess())
        {

            float3 start = localTransform.ValueRO.Position;
            localTransform.ValueRW.Position += math.forward(localTransform.ValueRO.Rotation) * deltaTime * projectileProperties.ValueRO.speed;
            if (localTransform.ValueRO.Position.y < 1) ecb.DestroyEntity(entity);
            float3 end = localTransform.ValueRO.Position;

            float3 direction = math.normalize(end - start);
            float distance = math.distance(start, end);

            var filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            };

            if (physicsWorld.CollisionWorld.CapsuleCast(
                start,
                end,
                0.7f,
                direction,
                distance,
                out ColliderCastHit hit,
                filter,
                QueryInteraction.Default))
            {
                ecb.DestroyEntity(hit.Entity);
                ecb.DestroyEntity(entity);
            }

        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}