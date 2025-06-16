using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Unity.Physics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class ShootingSystem : SystemBase
{
    private EntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecbSystem = World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();

        RequireForUpdate<WeaponProperties>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer();
        float deltaTime = SystemAPI.Time.DeltaTime;

        InputAction shootAction = InputSystem.actions.FindAction("Attack");
        float shootInput = shootAction?.ReadValue<float>() ?? 0f;

        float3 spawnPosition = new float3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y - 0.75f,
            Camera.main.transform.position.z
        );
        quaternion spawnRotation = quaternion.LookRotationSafe(Camera.main.transform.forward, math.up());

        Entities
            .ForEach((Entity entity, ref WeaponProperties weaponProps) =>
            {
                weaponProps.cooldownTimer -= deltaTime;

                if (shootInput != 0 && weaponProps.cooldownTimer <= 0f)
                {
                    weaponProps.cooldownTimer = weaponProps.cooldownAmount;

                    Entity spawned = ecb.Instantiate(weaponProps.projectile);
                    ecb.SetComponent(spawned, new LocalTransform
                    {
                        Position = spawnPosition,
                        Rotation = spawnRotation,
                        Scale = 1
                    });
                    ecb.SetComponent(spawned, new PhysicsVelocity
                    {
                        Linear = math.forward(spawnRotation) * weaponProps.speed,
                        Angular = float3.zero
                    });
                }
            }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
