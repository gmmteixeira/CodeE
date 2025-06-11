using UnityEngine;
using System;
using Unity.Mathematics;

public class SoundManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float globalVolume = 1;

    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume * globalVolume;
            s.source.pitch = s.pitch * UnityEngine.Random.Range(1 / (s.pitchRandomness + 1), s.pitchRandomness + 1);
            s.source.spatialBlend = s.spatial;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

[Serializable]
public class Sound
{
    public String name;
    public AudioClip audioClip;

    [Range(0f, 2f)]
    public float volume = 1;

    [Range(0.5f, 2f)]
    public float pitch = 1;

    [Range(0f, 1f)]
    public float pitchRandomness = 0;

    [Range(0f, 1f)]
    public float spatial = 1;
    public bool loop;
    public bool speedAffected = true;

    [HideInInspector]
    public AudioSource source;
}