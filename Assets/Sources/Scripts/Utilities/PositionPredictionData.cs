using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PositionPredictionData : IComponentData
{
    public float3 PredictedPosition;
}
