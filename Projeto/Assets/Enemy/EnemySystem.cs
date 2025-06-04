using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

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

        foreach ((RefRW<LocalTransform> localTransform, RefRO<EnemyProperties> enemyProperties, Entity entity)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyProperties>>().WithEntityAccess())
        {
            localTransform.ValueRW.Position += math.forward(localTransform.ValueRO.Rotation) * deltaTime * enemyProperties.ValueRO.forwardSpeed;

            float3 lTP = localTransform.ValueRO.Position;
            localTransform.ValueRW.Rotation = Quaternion.Slerp(
                localTransform.ValueRO.Rotation,
                Quaternion.LookRotation((GameVariables.staticVariables.player.transform.position - new Vector3(lTP.x, lTP.y, lTP.z)).normalized),
                Time.deltaTime * enemyProperties.ValueRO.turningSpeed);

            if (localTransform.ValueRO.Position.y < 1.5) localTransform.ValueRW.Position.y = 1.5f;

        }
    }
}