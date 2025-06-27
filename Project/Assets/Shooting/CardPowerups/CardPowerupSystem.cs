using Unity.Collections;
using Unity.Entities;

public partial class CardPowerupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Gather all entities with CardPowerup and their lifeTime
        var entities = new NativeList<Entity>(Allocator.Temp);
        var lifeTimes = new NativeList<float>(Allocator.Temp);

        Entities.ForEach((Entity entity, in CardPowerup cardPowerup) =>
        {
            entities.Add(entity);
            lifeTimes.Add(cardPowerup.lifeTime);
        }).Run();

        // Find indices of the 3 oldest (highest lifeTime) entities
        var oldestIndices = new NativeList<int>(Allocator.Temp);
        for (int i = 0; i < 3 && i < lifeTimes.Length; i++)
        {
            float maxLife = float.MinValue;
            int maxIdx = -1;
            for (int j = 0; j < lifeTimes.Length; j++)
            {
                if (!oldestIndices.Contains(j) && lifeTimes[j] > maxLife)
                {
                    maxLife = lifeTimes[j];
                    maxIdx = j;
                }
            }
            if (maxIdx != -1)
                oldestIndices.Add(maxIdx);
        }

        // Set active true for 3 oldest, false for others
        Entities.ForEach((Entity entity, ref CardPowerup cardPowerup) =>
        {
            int idx = entities.IndexOf(entity);
            cardPowerup.active = oldestIndices.Contains(idx);
            if (cardPowerup.active)
            {
                cardPowerup.lifeTime -= 1 * deltaTime;
            }
            if (cardPowerup.lifeTime <= 0)
                ecb.DestroyEntity(entity);
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        entities.Dispose();
        lifeTimes.Dispose();
        oldestIndices.Dispose();
    }
}