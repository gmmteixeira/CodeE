using System;
using Unity.Entities;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    public GameObject[] powerupPrefabs;
    private class GameSingletonBaker : Baker<GameSingleton>
    {
        public override void Bake(GameSingleton authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var buffer = AddBuffer<PowerupPrefabBufferElement>(entity);
            AddComponent(entity, new GameComponentData
            {
                score = 0,
            });
            foreach (var prefab in authoring.powerupPrefabs)
            {
                buffer.Add(new PowerupPrefabBufferElement
                {
                    Prefab = GetEntity(prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
public struct GameComponentData : IComponentData
{
    public int score;

}

[InternalBufferCapacity(4)]
public struct PowerupPrefabBufferElement : IBufferElementData
{
    public Entity Prefab;
}