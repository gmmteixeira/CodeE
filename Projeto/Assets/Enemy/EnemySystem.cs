using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial struct EnemySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRW<LocalTransform> localTransform, RefRW<EnemyProperties> enemyProperties, RefRO<LocalToWorld> localToWorld, Entity entity)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyProperties>, RefRO<LocalToWorld>>().WithEntityAccess())
        {
            localTransform.ValueRW.Position += localToWorld.ValueRO.Forward * deltaTime;

            float3 lTP = localTransform.ValueRO.Position;
            localTransform.ValueRW.Rotation = Quaternion.Slerp(
                localTransform.ValueRO.Rotation,
                Quaternion.LookRotation((Game.player.transform.position - new Vector3(lTP.x,lTP.y,lTP.z)).normalized),
                Time.deltaTime);

        }
    }
}