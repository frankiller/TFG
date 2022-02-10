using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class IslandPrefabBlobAssetAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity islandSpawnerEntity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var islandPrefabBlobAsset = ref blobBuilder.ConstructRoot<IslandPrefabBlobAsset>();
        var islandPrefabArray = blobBuilder.Allocate(ref islandPrefabBlobAsset.IslandPrefabArray, 1);

        var buffer = dstManager.GetBuffer<IslandPrefabBuffer>(islandSpawnerEntity);
        for (int i = 0; i < 1; i++)
        {
            islandPrefabArray[i] = new IslandPrefab
            {
                Value = buffer[i].Value,
                CannonPosition = buffer[i].CannonPosition
            };
        }

        var islandPrefabBlobAssetReference = blobBuilder.CreateBlobAssetReference<IslandPrefabBlobAsset>(Allocator.Persistent);

        dstManager.AddComponentData(islandSpawnerEntity, new IslandPrefabData
        {
            BlobAssetReference = islandPrefabBlobAssetReference,
            NextPrefabIndex = 0
        });
    }
}
