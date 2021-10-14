using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CameraTargetData : IComponentData
{
    public float3 Position;
    public quaternion Rotation;
    public float3 Offset;
}
