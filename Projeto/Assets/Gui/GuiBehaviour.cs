using TMPro;
using Unity.Entities;
using UnityEngine;

public class GuiBehaviour : MonoBehaviour
{
    public TextMeshProUGUI score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null)
        {
            var entityManager = world.EntityManager;
            try
            {
                if (entityManager.Exists(entityManager.CreateEntityQuery(typeof(GameData)).GetSingletonEntity()))
                {
                    var gameData = entityManager.CreateEntityQuery(typeof(GameData)).GetSingleton<GameData>();
                    score.text = gameData.score.ToString();
                }
            }
            catch (System.Exception)
            {
                score.text = 0.ToString();
            }
        }
    }

}
