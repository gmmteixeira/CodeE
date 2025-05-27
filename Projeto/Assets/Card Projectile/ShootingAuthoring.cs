using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;

public class ShootingAuthoring : MonoBehaviour
{

    public GameObject prefab;
    public float inacuracy;
    public int numberOfPorjectiles;
    public float shotCooldown;
    public float cooldownTimer;
    

    private class Baker : Baker<ShootingAuthoring>
    {
        public override void Bake(ShootingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootingProperties
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                inacuracy = authoring.inacuracy,
                numberOfPorjectiles = authoring.numberOfPorjectiles,
                shotCooldown = authoring.shotCooldown,
                cooldownTimer = authoring.cooldownTimer
            });
        }
    }
}

public struct ShootingProperties : IComponentData
{
    public Entity prefab;
    public float inacuracy;
    public int numberOfPorjectiles;
    public float shotCooldown;
    public float cooldownTimer;
}