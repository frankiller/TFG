using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CannonShootData : IComponentData
{
    public float3 Direction;
    public float Force;
}
