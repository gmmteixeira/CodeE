using UnityEngine;
using Unity.Entities;
using Unity.Collections;

/// <summary>
/// Centralized MonoBehaviour that reads ECS sound requests
/// and plays AudioSources accordingly.
/// </summary>
public class SoundManagerMB : MonoBehaviour
{
    public SoundDatabase soundDatabase;

    private EntityManager entityManager;
    private EntityQuery oneShotQuery, followQuery;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        oneShotQuery = entityManager.CreateEntityQuery(typeof(SoundRequest));
        followQuery  = entityManager.CreateEntityQuery(typeof(SoundEmitterRequest));
    }

    private void Update()
    {
        HandleOneShotRequests();
        HandleFollowRequests();
    }

    /// <summary>
    /// Plays one-shot sounds at fixed positions.
    /// </summary>
    private void HandleOneShotRequests()
    {
        var requests = oneShotQuery.ToComponentDataArray<SoundRequest>(Allocator.TempJob);
        var entities = oneShotQuery.ToEntityArray(Allocator.TempJob);

        for (int i = 0; i < requests.Length; i++)
        {
            var sound = soundDatabase.GetSound(requests[i].soundName.ToString());
            if (sound != null)
                PlayOneShot(sound, requests[i].position);

            entityManager.DestroyEntity(entities[i]);
        }

        requests.Dispose();
        entities.Dispose();
    }

    /// <summary>
    /// Plays sounds that follow an ECS entity.
    /// </summary>
    private void HandleFollowRequests()
    {
        var requests = followQuery.ToComponentDataArray<SoundEmitterRequest>(Allocator.TempJob);
        var entities = followQuery.ToEntityArray(Allocator.TempJob);

        for (int i = 0; i < requests.Length; i++)
        {
            var sound = soundDatabase.GetSound(requests[i].soundName.ToString());
            if (sound != null)
                PlayFollowing(sound, requests[i].followEntity);

            entityManager.DestroyEntity(entities[i]);
        }

        requests.Dispose();
        entities.Dispose();
    }

    private void PlayOneShot(Sound sound, Vector3 position)
    {
        GameObject go = new GameObject("OneShot_" + sound.name);
        var audio = go.AddComponent<AudioSource>();
        audio.clip = sound.audioClip;
        audio.volume = sound.volume;
        audio.pitch = sound.pitch;
        audio.spatialBlend = sound.spatial;
        audio.loop = sound.loop;
        go.transform.position = position;

        audio.Play();
        if (!sound.loop) Destroy(go, sound.audioClip.length / sound.pitch);
    }

    private void PlayFollowing(Sound sound, Entity target)
    {
        GameObject go = new GameObject("FollowSound_" + sound.name);
        var audio = go.AddComponent<AudioSource>();
        var tracker = go.AddComponent<SoundEmitterTracker>();

        tracker.entity = target;
        tracker.entityManager = entityManager;

        audio.clip = sound.audioClip;
        audio.volume = sound.volume;
        audio.pitch = sound.pitch;
        audio.spatialBlend = sound.spatial;
        audio.loop = sound.loop;

        audio.Play();
        if (!sound.loop) Destroy(go, sound.audioClip.length / sound.pitch);
    }
}
