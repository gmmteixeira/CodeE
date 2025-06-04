using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEditor.Search;

public partial struct ShootingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShootingProperties>();
    }

    public void OnUpdate(ref SystemState state)
    {

        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach ((RefRW<LocalTransform> localTransform, RefRO<ShootingProperties> shootingProperties, Entity entity)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ShootingProperties>>().WithEntityAccess())
        {
            var entityManager = state.EntityManager;
            InputAction shoot = InputSystem.actions.FindAction("Attack");
            if (shoot.ReadValue<float>() != 0)
            {
                Entity spawnedEntity = entityManager.Instantiate(shootingProperties.ValueRO.projectile);
                entityManager.SetComponentData(spawnedEntity, new LocalTransform
                {
                    Scale = 1,
                    Position = new float3(Camera.main.transform.position.x, Camera.main.transform.position.y - 0.75f, Camera.main.transform.position.z),
                    Rotation = quaternion.LookRotationSafe(Camera.main.transform.forward, math.up())
                });
            }
        }


    }
}