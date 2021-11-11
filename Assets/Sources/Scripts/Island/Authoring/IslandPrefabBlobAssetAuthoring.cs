using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class IslandPrefabBlobAssetAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> IslandPrefabs;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var islandPrefabBlobAsset = ref blobBuilder.ConstructRoot<IslandPrefabBlobAsset>();
        var islandPrefabArray = blobBuilder.Allocate(ref islandPrefabBlobAsset.IslandPrefabArray, 6);

        for (int i = 0; i < IslandPrefabs.Count; i++)
        {
            islandPrefabArray[i].Value = conversionSystem.GetPrimaryEntity(IslandPrefabs[i].gameObject);
            islandPrefabArray[i].CannonPosition = IslandPrefabs[i].transform.GetChild(0).localPosition;
        }

        var islandPrefabBlobAssetReference = blobBuilder.CreateBlobAssetReference<IslandPrefabBlobAsset>(Allocator.Persistent);

        dstManager.AddComponentData(entity, new IslandPrefabData
        {
            BlobAssetReference = islandPrefabBlobAssetReference,
            NextPrefab = islandPrefabArray[0]
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.AddRange(IslandPrefabs);
}
