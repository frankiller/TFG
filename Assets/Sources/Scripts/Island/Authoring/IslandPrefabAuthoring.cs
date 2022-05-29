using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
public class IslandPrefabAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Serializable]
    public struct IslandPrefab
    {
        public GameObject island;
        public float3 cannonPosition;
    }

    public IslandPrefab[] IslandPrefabs;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.AddRange(IslandPrefabs.Select(islandPrefab => islandPrefab.island));


    public void Convert(Entity islandSpawnerEntity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var islandPrefabBuffer = dstManager.AddBuffer<IslandPrefabBuffer>(islandSpawnerEntity);
        
        foreach (var islandPrefab in IslandPrefabs)
        {
            var entity = conversionSystem.GetPrimaryEntity(islandPrefab.island);
            //dstManager.SetName(entity, "IslandPrefab");
            islandPrefabBuffer.Add(new IslandPrefabBuffer
            {
                Value = entity,
                CannonPosition = islandPrefab.cannonPosition
            });
        }
    }
}
