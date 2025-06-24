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
        PlayerSingletonData playerData = default;
        bool hasPlayerData = SystemAPI.HasSingleton<PlayerSingletonData>();
        if (hasPlayerData)
        {
            playerData = SystemAPI.GetSingleton<PlayerSingletonData>();
        }
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
        // If your model's forward is +Y, rotate -90° around X to align Z+ to Y+
        quaternion projectileSpawnRotation = math.mul(spawnRotation, quaternion.RotateX(-math.radians(90f)));

        // Singleton pattern: get the only entity with WeaponProperties
        EntityQuery weaponQuery = GetEntityQuery(ComponentType.ReadWrite<WeaponProperties>());
        if (weaponQuery.CalculateEntityCount() == 0)
            return;

        Entity weaponEntity = weaponQuery.GetSingletonEntity();
        var weaponProps = EntityManager.GetComponentData<WeaponProperties>(weaponEntity);

        weaponProps.cooldownTimer -= deltaTime;
        bool fired = false;
        if (shootInput != 0 && weaponProps.cooldownTimer <= 0f && playerData.isAlive)
        {
            weaponProps.cooldownTimer = weaponProps.cooldownAmount;
            Entity sound = ecb.Instantiate(weaponProps.soundEffect);
            ecb.SetComponent(sound, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = spawnRotation,
                Scale = 1.0f
            });
            ecb.AddComponent(sound, new Expiration
            {
                timeToLive = 1.5f
            });
            Entity spawned = ecb.Instantiate(weaponProps.projectile);
            ecb.SetComponent(spawned, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = projectileSpawnRotation,
                Scale = 1.25f
            });
            ecb.SetComponent(spawned, new PhysicsVelocity
            {
                Linear = math.mul(projectileSpawnRotation, new float3(0, -1, 0)) * weaponProps.speed,
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
