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
        var camTransform = Camera.main.transform;
        PlayerSingletonData playerData = default;
        bool hasPlayerData = SystemAPI.HasSingleton<PlayerSingletonData>();
        if (hasPlayerData)
        {
            playerData = SystemAPI.GetSingleton<PlayerSingletonData>();
        }
        var ecb = _ecbSystem.CreateCommandBuffer();
        float deltaTime = SystemAPI.Time.DeltaTime;

        InputAction shootAction = InputSystem.actions.FindAction("Attack");
        InputAction altShootAction = InputSystem.actions.FindAction("AltAttack");
        float shootInput = shootAction?.ReadValue<float>() ?? 0f;
        var game = SystemAPI.GetSingleton<GameComponentData>();

        float3 forwardOffset = camTransform.forward * 0.3f + camTransform.up * -0.5f;
        float3 spawnPosition = new float3(
            camTransform.position.x,
            camTransform.position.y,
            camTransform.position.z
        ) + forwardOffset;
        quaternion spawnRotation = quaternion.LookRotationSafe(camTransform.forward, math.up());
        quaternion projectileSpawnRotation = math.mul(spawnRotation, quaternion.RotateX(-math.radians(90f)));

        EntityQuery weaponQuery = GetEntityQuery(ComponentType.ReadWrite<WeaponProperties>());
        if (weaponQuery.CalculateEntityCount() == 0)
            return;

        Entity weaponEntity = weaponQuery.GetSingletonEntity();
        var weaponProps = EntityManager.GetComponentData<WeaponProperties>(weaponEntity);

        weaponProps.cooldownTimer -= deltaTime;
        if (weaponProps.powerupDrain > 0f && weaponProps.powerupLevel > 0)
        {
            weaponProps.powerupDrain -= 0.5f * deltaTime * (0.5f + weaponProps.powerupLevel * 0.5f);
        }
        if (weaponProps.powerupDrain <= 0f && weaponProps.powerupLevel > 0)
        {
            weaponProps.powerupLevel -= 1;
            weaponProps.powerupDrain = 20f;
        }
        weaponProps.powerupDrain -= deltaTime * (0.5f + weaponProps.powerupLevel * 0.5f);
        bool fired = false;
        bool altFired = false;
        if (altShootAction.triggered && weaponProps.powerupLevel > 0 && (weaponProps.powerupDrain >= 19.5f || weaponProps.powerupLevel > 1) && (game.tutorial == 0 || game.tutorial >= 8))
        {
            weaponProps.powerupLevel -= 1;
            weaponProps.cooldownTimer = 0.5f;
            float3 laserForwardOffset = camTransform.forward * 100.5f + camTransform.up * -0.5f;
            float3 laserSpawnPosition = new float3(
                camTransform.position.x,
                camTransform.position.y,
                camTransform.position.z
            ) + laserForwardOffset;
            Entity laser = ecb.Instantiate(weaponProps.laserPrefab);
            ecb.SetComponent(laser, new LocalTransform
            {
                Position = laserSpawnPosition,
                Rotation = projectileSpawnRotation,
                Scale = 1.0f
            });
            ecb.AddComponent(laser, new LaserProperties
            {
                damage = 100,
                activeTime = 0.3f
            });
            float3 laserScale = new float3(5, 100, 5);
            float4x4 laserTransform = float4x4.Scale(laserScale);
            ecb.AddComponent(laser, new PostTransformMatrix
            {
                Value = laserTransform
            });
            fired = true;
            altFired = true;
        }

        if (shootInput != 0 && weaponProps.cooldownTimer <= 0f && playerData.isAlive && (game.tutorial == 0 || game.tutorial >= 5))
        {
            float cooldown = 1;
            int projectileCount = 1;
            float spread = 1;
            int damage = 1;
            float explosion = 0;
            Entity effect = Entity.Null;

            switch (weaponProps.powerupLevel)
            {
                case 0:
                    cooldown = weaponProps.lvl0Cooldown;
                    projectileCount = weaponProps.lvl0ProjectileCount;
                    spread = weaponProps.lvl0Spread;
                    damage = weaponProps.lvl0Damage;
                    explosion = weaponProps.lvl0Explosion;
                    effect = weaponProps.shootEffectlvl0;
                    break;
                case 1:
                    cooldown = weaponProps.lvl1Cooldown;
                    projectileCount = weaponProps.lvl1ProjectileCount;
                    spread = weaponProps.lvl1Spread;
                    damage = weaponProps.lvl1Damage;
                    explosion = weaponProps.lvl1Explosion;
                    effect = weaponProps.shootEffectlvl1;
                    break;
                case 2:
                    cooldown = weaponProps.lvl2Cooldown;
                    projectileCount = weaponProps.lvl2ProjectileCount;
                    spread = weaponProps.lvl2Spread;
                    damage = weaponProps.lvl2Damage;
                    explosion = weaponProps.lvl2Explosion;
                    effect = weaponProps.shootEffectlvl2;
                    break;
                case 3:
                    cooldown = weaponProps.lvl3Cooldown;
                    projectileCount = weaponProps.lvl3ProjectileCount;
                    spread = weaponProps.lvl3Spread;
                    damage = weaponProps.lvl3Damage;
                    explosion = weaponProps.lvl3Explosion;
                    effect = weaponProps.shootEffectlvl3;
                    break;
                default: break;
            }

            weaponProps.cooldownTimer = cooldown;
            Entity effectInstance = Entity.Null;
            if (effect != Entity.Null)
            {
                effectInstance = ecb.Instantiate(effect);
                ecb.SetComponent(effectInstance, new LocalTransform
                {
                    Position = spawnPosition,
                    Rotation = spawnRotation,
                    Scale = 1.0f
                });
                ecb.AddComponent(effectInstance, new Expiration
                {
                    timeToLive = 1.5f
                });
            }
            for (int i = 0; i < projectileCount; i++)
            {
                // Biased circular spread (more projectiles near center)
                float angle = UnityEngine.Random.Range(0f, 360f);
                float radius = UnityEngine.Random.Range(0f, 1f) * spread;

                float pitch = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float yaw = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

                quaternion spreadRotation = math.mul(
                    quaternion.RotateX(math.radians(pitch)),
                    quaternion.RotateZ(math.radians(yaw))
                );
                quaternion randomizedRotation = math.mul(projectileSpawnRotation, spreadRotation);

                Entity spawned = ecb.Instantiate(weaponProps.projectile);
                ecb.SetComponent(spawned, new LocalTransform
                {
                    Position = spawnPosition,
                    Rotation = randomizedRotation,
                    Scale = 1.25f
                });
                ecb.AddComponent(spawned, new ProjectileDamageProperties
                {
                    damage = damage,
                    explosion = explosion,
                    explosionPrefab = weaponProps.explosionPrefab,
                    projectileHit = weaponProps.projectileHit
                });
                ecb.SetComponent(spawned, new PhysicsVelocity
                {
                    Linear = math.mul(randomizedRotation, new float3(0, -1, 0)) * weaponProps.speed,
                    Angular = float3.zero
                });
            }

            fired = true;
        }

        // Write back the modified struct
        EntityManager.SetComponentData(weaponEntity, weaponProps);
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            SystemAPI.SetSingleton(game);
        }
        if (fired)
            WeaponEvents.WeaponFired();

        if (altFired)
            WeaponEvents.WeaponAltFired();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}