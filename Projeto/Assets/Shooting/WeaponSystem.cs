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

        // Singleton pattern: get the only entity with WeaponProperties
        EntityQuery weaponQuery = GetEntityQuery(ComponentType.ReadWrite<WeaponProperties>());
        if (weaponQuery.CalculateEntityCount() == 0)
            return;

        Entity weaponEntity = weaponQuery.GetSingletonEntity();
        var weaponProps = EntityManager.GetComponentData<WeaponProperties>(weaponEntity);

        weaponProps.cooldownTimer -= deltaTime;

        bool fired = false;
        if (shootInput != 0 && weaponProps.cooldownTimer <= 0f)
        {
            weaponProps.cooldownTimer = weaponProps.cooldownAmount;
            Entity sound = ecb.Instantiate(weaponProps.soundEffect);
            ecb.AddComponent(sound, new Expiration
            {
                timeToLive = 1.5f
            });
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

            fired = true;
        }

        // Write back the modified struct
        EntityManager.SetComponentData(weaponEntity, weaponProps);

        if (fired)
            WeaponEvents.WeaponFired();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
