using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial class ExplosionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entities.ForEach((ref PhysicsMass mass, ref LocalTransform localTransform, in ExplosionProperties explosionProperties) =>
        {
            mass.InverseInertia = float3.zero;
            localTransform.Scale -= 20 * deltaTime;
            if (localTransform.Scale < 0)
            {
                localTransform.Scale = 0;
            }
        }).ScheduleParallel();
    }
}