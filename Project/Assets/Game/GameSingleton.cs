using System;
using Unity.Entities;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    private class GameSingletonBaker : Baker<GameSingleton>
    {
        public override void Bake(GameSingleton authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GameComponentData
            {
                score = 0,
            });
        }
    }
}
public struct GameComponentData : IComponentData
{
    public int score;
}