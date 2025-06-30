using Unity.Collections;
using Unity.Entities;

public partial class CardPowerupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Set active true for 3 oldest, false for others
        Entities.ForEach((Entity entity, ref CardPowerup cardPowerup) =>
        {
            if (cardPowerup.active)
            {
                cardPowerup.lifeTime -= 1 * deltaTime;
            }
            if (cardPowerup.lifeTime <= 0)
                ecb.DestroyEntity(entity);
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}