using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
public partial class LaserSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entities.WithAll<LaserProperties>().ForEach((ref LaserProperties laser, ref PhysicsMass mass, ref PostTransformMatrix postTransform) =>
        {
            mass.InverseInertia = float3.zero;
            laser.activeTime -= deltaTime;
            postTransform.Value.c0.x -= 10 * deltaTime;
            if (postTransform.Value.c0.x < 0)
            {
                postTransform.Value.c0.x = 0;
            }
            postTransform.Value.c2.z -= 10 * deltaTime;
            if (postTransform.Value.c2.z < 0)
            {
                postTransform.Value.c2.z = 0;
            }
        }).ScheduleParallel();
    }
}