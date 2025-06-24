using Unity.Collections;
using Unity.Entities;
public partial class ExpirationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();
        Entities.WithAll<Expiration>()
        .ForEach((Entity entity, int entityInQueryIndex, ref Expiration expiration) =>
        {
            expiration.timeToLive -= deltaTime;

            if (expiration.timeToLive <= 0)
            {
                ecbParallel.DestroyEntity(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

public struct Expiration : IComponentData
{
    public float timeToLive;
}