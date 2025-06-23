using UnityEngine;
using Unity.Entities;

public class PlayerSingleton : MonoBehaviour
{
    private bool isAlive = true; // Exposed to Inspector

    private class Baker : Baker<PlayerSingleton>
    {
        public override void Bake(PlayerSingleton authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerSingletonData
            {
                isAlive = authoring.isAlive, // Use Inspector value
            });
        }
    }
}
public struct PlayerSingletonData : IComponentData
{
    public bool isAlive;
}
