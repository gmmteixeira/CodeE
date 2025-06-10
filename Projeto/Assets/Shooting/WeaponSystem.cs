using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct ShootingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponProperties>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        foreach ((RefRW<WeaponProperties> shootingProperties, Entity entity)
        in SystemAPI.Query<RefRW<WeaponProperties>>().WithEntityAccess())
        {
            
            shootingProperties.ValueRW.cooldownTimer -= 1 * deltaTime;

            InputAction shoot = InputSystem.actions.FindAction("Attack");
            if (shoot.ReadValue<float>() != 0 && shootingProperties.ValueRO.cooldownTimer <= 0)
            {
                shootingProperties.ValueRW.cooldownTimer = shootingProperties.ValueRO.cooldownAmount;

                Entity spawnedEntity = entityManager.Instantiate(shootingProperties.ValueRO.projectile);
                entityManager.SetComponentData(spawnedEntity, new LocalTransform
                {
                    Position = new float3(Camera.main.transform.position.x, Camera.main.transform.position.y - 0.75f, Camera.main.transform.position.z),
                    Rotation = quaternion.LookRotationSafe(Camera.main.transform.forward, math.up()),
                    Scale = 1,
                });
            }
        }


    }
}