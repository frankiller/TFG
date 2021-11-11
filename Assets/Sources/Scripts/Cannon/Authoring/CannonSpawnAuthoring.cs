using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CannonSpawnSettings : ISpawnSettings, IComponentData
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
}

public class CannonSpawnAuthoring : SpawnObjectAuthoringBase<CannonSpawnSettings> { }
