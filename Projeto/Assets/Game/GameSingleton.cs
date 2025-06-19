using Unity.Entities;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    public int seed = 1;
    
    private class GameSingletonBaker : Baker<GameSingleton>
    {
        public override void Bake(GameSingleton authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GameData
            {
                score = 0,
                seed = authoring.seed
            });
        }
    }
}
public struct GameData : IComponentData
{
    public int score;
    public int seed;
}