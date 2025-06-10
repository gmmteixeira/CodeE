using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class ProjectileAuthoring : MonoBehaviour
{
    public float speed;

    private class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new ProjectileProperties
            {
                speed = authoring.speed
            });
        }
    }
}

public struct ProjectileProperties : IComponentData
{
    public float speed;
    
}