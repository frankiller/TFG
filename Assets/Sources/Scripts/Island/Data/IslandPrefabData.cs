using System;
using Unity.Entities;

[Serializable]
public struct IslandPrefabData : IComponentData
{
    public BlobAssetReference<IslandPrefabBlobAsset> BlobAssetReference;
    public int NextPrefabIndex;
}