using UnityEngine;
using Unity.Entities;
using Unity.Transforms;


public class SoundEmitterTracker : MonoBehaviour
{
    public Entity entity;
    public EntityManager entityManager;

    private void Update()
    {
        if (!entityManager.Exists(entity))
        {
            Destroy(gameObject);
            return;
        }

        if (entityManager.HasComponent<LocalToWorld>(entity))
        {
            var pos = entityManager.GetComponentData<LocalToWorld>(entity).Position;
            transform.position = pos;
        }
    }
}
