using Unity.Entities;
using UnityEngine;

public class ArenaColliderAuthoring : MonoBehaviour
{
    public class Baker : Baker<ArenaColliderAuthoring>
    {
        public override void Bake(ArenaColliderAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ArenaCollider());
        }
    }
}

public struct ArenaCollider : IComponentData {}