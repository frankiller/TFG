using Unity.Entities;
using Unity.Mathematics;

public struct IslandPrefab
{
    public Entity Value;
    public float3 CannonPosition;
}

public struct IslandPrefabBlobAsset
{
    public BlobArray<IslandPrefab> IslandPrefabArray;
}