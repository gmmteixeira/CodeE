using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct SoundRequest : IComponentData
{
    public FixedString64Bytes soundName;
    public float3 position;
}

public struct SoundEmitterRequest : IComponentData
{
    public FixedString64Bytes soundName;
    public Entity followEntity;
}