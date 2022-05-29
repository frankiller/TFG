using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrajectoryPredictionData : IComponentData
{
    public float3 Value;
}
