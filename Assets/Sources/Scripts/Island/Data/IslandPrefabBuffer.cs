using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(6)]
public struct IslandPrefabBuffer : IBufferElementData
{
    public Entity Value;
    public float3 CannonPosition;
}
