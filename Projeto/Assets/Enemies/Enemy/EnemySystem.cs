using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial class EnemySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();
        Vector3 playerPosition = SystemAPI.GetSingleton<PlayerPosition>().vector3;
        int score = 0;
        if (SystemAPI.HasSingleton<GameData>())
        {
            score = SystemAPI.GetSingleton<GameData>().score;
        }

        Entities
        .WithAll<PhysicsVelocity>()
        .ForEach((ref PhysicsMass mass, ref PhysicsVelocity velocity, ref LocalTransform localTransform, in HomingBoidProperties homingBoidProperties) =>
        {
            mass.InverseInertia = float3.zero;
            float3 forward = math.forward(localTransform.Rotation);
            velocity.Linear = forward * homingBoidProperties.forwardSpeed;

            float3 lTP = localTransform.Position;
            localTransform.Rotation = Quaternion.Slerp(
                localTransform.Rotation,
                Quaternion.LookRotation((playerPosition - new Vector3(lTP.x, lTP.y, lTP.z)).normalized),
                deltaTime * homingBoidProperties.turningSpeed);

        }).ScheduleParallel();


        Entities.WithoutBurst().ForEach((Entity entity, int entityInQueryIndex, AudioSource audioSource, in HealthProperties damageProperties) =>
        {
            if (damageProperties.health <= 0)
            {
                Entity deathEntity = ecbParallel.Instantiate(entityInQueryIndex, damageProperties.deathEffect);
                ecbParallel.SetComponent(entityInQueryIndex, deathEntity, new LocalTransform
                {
                    Position = SystemAPI.GetComponent<LocalTransform>(entity).Position,
                });
                ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                score += 1;
            }

        }).Run();
        if (SystemAPI.HasSingleton<GameData>())
        {
            SystemAPI.SetSingleton(new GameData { score = score });
        }

        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}