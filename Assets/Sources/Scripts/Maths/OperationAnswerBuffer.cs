using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
struct OperationAnswerBuffer : IBufferElementData
{
    public int Value;
    public float3 Position;
    public quaternion Rotation;
    public bool IsCorrect;
}
