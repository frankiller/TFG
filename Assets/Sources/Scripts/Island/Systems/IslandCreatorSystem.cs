using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class IslandCreatorSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<TrajectoryPredictionData>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var islandPrefabData =  GetSingleton<IslandPrefabData>();
        var islandSpawnerEntity = GetSingletonEntity<IslandSpawnerTag>();
        var islandSpawnOffset = GetSingleton<IslandSpawnOffsetAuthoring>().Value;

        //var random = new Random(1).NextInt(0, islandPrefabData.BlobAssetReference.Value.IslandPrefabBuffer.Length);

        var islandPrefab = islandPrefabData.BlobAssetReference.Value.IslandPrefabArray[islandPrefabData.NextPrefabIndex].Value;
        var cannonPosition = islandPrefabData.BlobAssetReference.Value.IslandPrefabArray[islandPrefabData.NextPrefabIndex].CannonPosition;

        islandPrefabData.NextPrefabIndex++;

        Entities.
            WithName("IslandCreatorSystem").
            ForEach((Entity entity, in TrajectoryPredictionData positionPredictionData) =>
            {
                var newPosition = positionPredictionData.Value - islandSpawnOffset;
                ecb.AddComponent(islandSpawnerEntity, new IslandSpawnSettings
                {
                    Prefab = islandPrefab,
                    Position = newPosition,
                    Rotation = quaternion.identity
                });

                ecb.AddComponent(islandSpawnerEntity, new CannonPositionData { Value = cannonPosition + newPosition });

                ecb.RemoveComponent<TrajectoryPredictionData>(entity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class UpdateIslandNextPrefabSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<TrajectoryPredictionData>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var islandPrefabData = GetSingleton<IslandPrefabData>();

        Entities.
            WithChangeFilter<IslandSpawnSettings>().
            ForEach((Entity e) =>
            {
                islandPrefabData.NextPrefabIndex = new Random(1).NextInt(0, islandPrefabData.BlobAssetReference.Value.IslandPrefabArray.Length);

                ecb.SetComponent(e, islandPrefabData);
            }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
