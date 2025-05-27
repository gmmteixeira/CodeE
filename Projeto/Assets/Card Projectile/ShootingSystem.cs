using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public partial struct ShootingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShootingProperties>();
    }
    public void OnUpdate(ref SystemState state)
    {
        ShootingProperties shootingProperties = SystemAPI.GetSingleton<ShootingProperties>();
        var entityManager = state.EntityManager;

        //if (shootingProperties.cooldownTimer > 0) shootingProperties.cooldownTimer -= 1 * SystemAPI.Time.DeltaTime;

        if (Input.GetMouseButton(0) /*&& shootingProperties.cooldownTimer <= 0*/)
        {
            //shootingProperties.cooldownTimer = shootingProperties.shotCooldown;

            for (int i = 0; i < shootingProperties.numberOfPorjectiles; i++)
            {
                float inacuracy = shootingProperties.inacuracy;

                Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-inacuracy, inacuracy),
                UnityEngine.Random.Range(-inacuracy, inacuracy),
                UnityEngine.Random.Range(-inacuracy, inacuracy)
                );


                Entity spawnedEntity = entityManager.Instantiate(shootingProperties.prefab);
                entityManager.SetComponentData(spawnedEntity, new LocalTransform
                {
                    Scale = 1,
                    Position = new float3(Camera.main.transform.position.x, Camera.main.transform.position.y - 0.75f, Camera.main.transform.position.z),
                    Rotation = quaternion.LookRotationSafe((Camera.main.transform.forward + randomOffset).normalized, math.up())
                });
            }

        }


    }
}
