using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;


public partial class ProjectyleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();
        
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref LocalTransform localTransform, in ProjectileProperties projectileProperties) =>
        {
            localTransform.Position += deltaTime * projectileProperties.speed * math.forward(localTransform.Rotation);
            if (math.distance(localTransform.Position, new float3(0,0,0)) > 200) {ecbParallel.DestroyEntity(entityInQueryIndex, entity);}

        }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}