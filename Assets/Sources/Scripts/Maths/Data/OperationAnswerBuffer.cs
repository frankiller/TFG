using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct OperationAnswerBuffer : IBufferElementData
{
    public float Value;
    public float3 Position;
    public quaternion Rotation;
    public bool IsCorrect;
}
