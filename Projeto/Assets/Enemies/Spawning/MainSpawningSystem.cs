using Unity.Entities;
using UnityEngine;

public partial class MainSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        ref var mainSpawning = ref SystemAPI.GetSingletonRW<MainSpawningProperties>().ValueRW;
        ref var scene = ref SystemAPI.GetSingletonRW<GameData>().ValueRW;

        mainSpawning.cooldown -= deltaTime;
        if (mainSpawning.cooldown <= 0f)
        {
            mainSpawning.cooldown = 
        }
    }
}
