using System;
using Unity.Entities;

[Serializable]
public struct IslandPrefabBuffer : IBufferElementData
{
    public Entity Value;
}
