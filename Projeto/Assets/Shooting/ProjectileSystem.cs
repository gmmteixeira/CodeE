using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;

[BurstCompile]
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
        var entityManager = state.EntityManager;

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach ((RefRW<LocalTransform> localTransform, RefRO<ProjectileProperties> projectileProperties, Entity entity)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ProjectileProperties>>().WithEntityAccess())
        {

            float3 start = localTransform.ValueRO.Position;

            localTransform.ValueRW.Position += math.forward(localTransform.ValueRO.Rotation) * deltaTime * projectileProperties.ValueRO.speed;

            if (localTransform.ValueRO.Position.y < 0.25 || math.distance(localTransform.ValueRO.Position, new float3(0, 0, 0)) > 120) ecb.DestroyEntity(entity);

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
                0.5f,
                direction,
                distance,
                out ColliderCastHit hit,
                filter,
                QueryInteraction.Default))
            {
                var enemyProperties = entityManager.GetComponentData<EnemyProperties>(hit.Entity);
                enemyProperties.health -= 1;
                entityManager.SetComponentData(hit.Entity, enemyProperties);
                ecb.DestroyEntity(entity);
            }

        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}