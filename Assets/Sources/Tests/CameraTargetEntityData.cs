using System;
using Unity.Entities;

[Serializable]
public struct CameraTargetEntityData : IComponentData
{
    public Entity Value;
}
