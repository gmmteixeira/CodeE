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
                tutorial = 8,
                tutorialTimer = 1
            });
        }
    }
}
public struct GameComponentData : IComponentData
{
    public int score;
    public int tutorial;
    /*
    0 = normal game
    1 = tutorial start
    2 = fps controls
    3 = basic enemy
    4 = jumping
    5 = shooting
    6 = powerup pick up
    7 = powerup level up
    8 = laser
    9 = death
    */
    public float tutorialTimer;
}