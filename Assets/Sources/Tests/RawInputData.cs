using System;
using Unity.Entities;

[Serializable]
public struct RawInputData : IComponentData
{
    public float XInput;
    public float YInput;
}
