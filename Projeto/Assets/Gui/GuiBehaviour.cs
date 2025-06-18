using TMPro;
using Unity.Entities;
using UnityEngine;

public class GuiBehaviour : MonoBehaviour
{
    public TextMeshProUGUI score;
    EntityManager _entityManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = _entityManager.CreateArchetype(typeof(GameData));
        _entityManager.AddComponentData(_entityManager.CreateEntity(archetype), new GameData
        {
            score = 0
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (_entityManager != null && _entityManager.HasComponent<GameData>(_entityManager.CreateEntityQuery(typeof(GameData)).GetSingletonEntity()))
        {
            var GameData = _entityManager.GetComponentData<GameData>(_entityManager.CreateEntityQuery(typeof(GameData)).GetSingletonEntity());
            score.text = GameData.score.ToString();
        }
    }

}
