using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[Serializable]
public struct IslandSpawnSettings : ISpawnSettings, IComponentData
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
}

public class IslandSpawnAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity islandSpawner, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.SetName(islandSpawner, $"{name}");

        var nextPrefab = dstManager.GetComponentData<IslandPrefabData>(islandSpawner).NextPrefab;
        var islandSpawnOffset = dstManager.GetComponentData<IslandSpawnOffsetAuthoring>(islandSpawner).Value;
        var newPosition = dstManager.GetComponentData<LocalToWorld>(nextPrefab.Value).Position - islandSpawnOffset;
        
        var spawnSettings = new IslandSpawnSettings
        {
            Prefab = nextPrefab.Value,
            Position = newPosition,
            Rotation = dstManager.GetComponentData<LocalToWorld>(nextPrefab.Value).Rotation
        };

        dstManager.AddComponentData(islandSpawner, new CannonPositionData { Value = nextPrefab.CannonPosition + newPosition });
        dstManager.AddComponentData(islandSpawner, spawnSettings);
    }
}

