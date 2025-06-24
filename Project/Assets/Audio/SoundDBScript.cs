using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Database")]
public class SoundDatabase : ScriptableObject
{
    public List<Sound> sounds;

    /// <summary> Retrieves a sound by name from the list. </summary>
    public Sound GetSound(string name) =>
        sounds.Find(s => s.name == name);
}

[Serializable]
public class Sound
{
    public String name;
    public AudioClip audioClip;

    [Range(0f, 2f)]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    public float pitch = 1f;

    [Range(0f, 1f)]
    public float spatial = 1f;
    public bool loop;
}
