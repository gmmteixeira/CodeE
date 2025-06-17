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
        EntityArchetype archetype = _entityManager.CreateArchetype(typeof(GuiProperties));
        _entityManager.AddComponentData(_entityManager.CreateEntity(archetype), new GuiProperties
        {
            score = 0
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (_entityManager != null && _entityManager.HasComponent<GuiProperties>(_entityManager.CreateEntityQuery(typeof(GuiProperties)).GetSingletonEntity()))
        {
            var guiProperties = _entityManager.GetComponentData<GuiProperties>(_entityManager.CreateEntityQuery(typeof(GuiProperties)).GetSingletonEntity());
            score.text = guiProperties.score.ToString();
        }
    }

}
