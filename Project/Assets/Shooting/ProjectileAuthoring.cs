using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class ProjectileAuthoring : MonoBehaviour
{
    public float distanceLimit;
    public float damage;
    public float explosion;
    public GameObject explosionPrefab;

    private class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new ProjectileFlightProperties { distanceLimit = authoring.distanceLimit });
        }
    }
}

public struct ProjectileFlightProperties : IComponentData {public float distanceLimit;}
public struct ProjectileDamageProperties : IComponentData 
{
    public float damage;
    public float explosion;
    public Entity explosionPrefab;
}