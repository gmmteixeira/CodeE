using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Physics;

[BurstCompile]
public partial struct EnemySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyProperties>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach ((RefRW<LocalTransform> localTransform, RefRO<EnemyProperties> enemyProperties, Entity entity)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyProperties>>().WithEntityAccess())
        {
            if (enemyProperties.ValueRO.health <= 0) ecb.DestroyEntity(entity);
            localTransform.ValueRW.Position += math.forward(localTransform.ValueRO.Rotation) * deltaTime * enemyProperties.ValueRO.forwardSpeed;

            float3 lTP = localTransform.ValueRO.Position;
            localTransform.ValueRW.Rotation = Quaternion.Slerp(
                localTransform.ValueRO.Rotation,
                Quaternion.LookRotation((Game.staticVariables.player.transform.position - new Vector3(lTP.x, lTP.y, lTP.z)).normalized),
                deltaTime * enemyProperties.ValueRO.turningSpeed);

            if (localTransform.ValueRO.Position.y < 1) localTransform.ValueRW.Position.y = 1;

            var filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            };

            if (physicsWorld.SphereCast(
                localTransform.ValueRO.Position,
                2f,
                new Vector3(0, 0, 0),
                0,
                out ColliderCastHit hit,
                filter,
                QueryInteraction.Default
            ))
            {
                localTransform.ValueRW.Rotation = Quaternion.Slerp(
                    localTransform.ValueRO.Rotation,
                    Quaternion.LookRotation((new Vector3(lTP.x, lTP.y, lTP.z) - new Vector3(hit.Position.x, hit.Position.y, hit.Position.z)).normalized),
                    deltaTime * 5 / (math.distance(localTransform.ValueRO.Position, hit.Position) * 2));
            }


        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}