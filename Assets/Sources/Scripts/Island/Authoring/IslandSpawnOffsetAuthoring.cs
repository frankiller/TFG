using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct IslandSpawnOffsetAuthoring : IComponentData
{
    public float3 Value;
}