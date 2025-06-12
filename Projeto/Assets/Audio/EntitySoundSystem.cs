using System;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public partial struct EntitySoundSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        foreach ((RefRO<CreateSound> createSound, Entity entity) in SystemAPI.Query<RefRO<CreateSound>>().WithEntityAccess())
        {
            foreach (FixedString32Bytes sound in createSound.ValueRO.soundNames)
            {
                
            }
        }
    }
}

public struct CreateSound : IComponentData { public FixedString32Bytes[] soundNames; }
public struct DestroySound : IComponentData { public FixedString32Bytes soundName; }