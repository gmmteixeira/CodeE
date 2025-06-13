using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;


public partial class EnemySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Vector3 playerPosition = SystemAPI.GetSingleton<PlayerPosition>().vector3;
        Entities.ForEach((ref LocalTransform localTransform, in HomingBoidProperties homingBidProperties) =>
        {
            localTransform.Position += deltaTime * homingBidProperties.forwardSpeed * math.forward(localTransform.Rotation);

            float3 lTP = localTransform.Position;
            localTransform.Rotation = Quaternion.Slerp(
                localTransform.Rotation,
                Quaternion.LookRotation((playerPosition - new Vector3(lTP.x, lTP.y, lTP.z)).normalized),
                deltaTime * homingBidProperties.turningSpeed);

            if (localTransform.Position.y < 1) localTransform.Position.y = 1;

        }).ScheduleParallel();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}