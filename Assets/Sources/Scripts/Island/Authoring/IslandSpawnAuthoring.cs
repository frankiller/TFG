using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[Serializable]
public struct IslandSpawnSettings : ISpawnSettings, IComponentData
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
}

[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
[UpdateAfter(typeof(IslandPrefabBlobAssetAuthoring))]
public class IslandSpawnAuthoring : GameObjectConversionSystem
{
    private EntityManager _entityManager;
    private EntityQuery _islandSpawnerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _islandSpawnerQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<IslandSpawnerTag>());
    }

    protected override void OnUpdate()
    {
        if (_islandSpawnerQuery.IsEmptyIgnoreFilter) return;

        var islandSpawnerEntity = _islandSpawnerQuery.GetSingletonEntity();
        var islandPrefabData = _entityManager.GetComponentData<IslandPrefabData>(islandSpawnerEntity);
        var nextPrefabIndex = islandPrefabData.NextPrefabIndex;
        var nextPrefab = islandPrefabData.BlobAssetReference.Value.IslandPrefabArray[nextPrefabIndex].Value;
        var islandSpawnOffset = _entityManager.GetComponentData<IslandSpawnOffsetAuthoring>(islandSpawnerEntity).Value;
        var newPosition = _entityManager.GetComponentData<LocalToWorld>(nextPrefab).Position - islandSpawnOffset;

        _entityManager.AddComponentData(islandSpawnerEntity, new CannonPositionData
        {
            Value = islandPrefabData.BlobAssetReference.Value.IslandPrefabArray[nextPrefabIndex].CannonPosition + newPosition
        });
        
        var spawnSettings = new IslandSpawnSettings
        {
            Prefab = nextPrefab,
            Position = newPosition,
            Rotation = _entityManager.GetComponentData<LocalToWorld>(nextPrefab).Rotation
        };

        _entityManager.AddComponentData(islandSpawnerEntity, spawnSettings);
        
    }
}

