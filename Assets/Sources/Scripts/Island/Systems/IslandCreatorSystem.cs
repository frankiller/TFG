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

        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<PositionPredictionData>()));
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var islandSpawnerEntity = GetSingletonEntity<IslandSpawnerTag>();
        var islandSpawnOffset = GetSingleton<IslandSpawnOffsetAuthoring>().Value;
        ref var islandPrefabArray = ref GetSingleton<IslandPrefabData>().BlobAssetReference.Value.IslandPrefabArray;
        var random = new Random(1).NextInt(0, islandPrefabArray.Length);
        var islandPrefab = islandPrefabArray[random].Value;
        var cannonPosition = islandPrefabArray[random].CannonPosition;

        Entities.
            WithName("IslandCreatorSystem").
            ForEach((Entity entity, in PositionPredictionData positionPredictionData) =>
            {
                var newPosition = positionPredictionData.Value - islandSpawnOffset;
                ecb.AddComponent(islandSpawnerEntity, new IslandSpawnSettings
                {
                    Prefab = islandPrefab,
                    Position = newPosition,
                    Rotation = quaternion.identity
                });

                ecb.AddComponent(islandSpawnerEntity, new CannonPositionData { Value = cannonPosition + newPosition });

                ecb.RemoveComponent<PositionPredictionData>(entity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class UpdateIslandPrefabToSpawnSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var islandPrefabData = GetSingleton<IslandPrefabData>();
        ref var islandPrefabArray = ref islandPrefabData.BlobAssetReference.Value.IslandPrefabArray;
        var random = new Random(1).NextInt(0, islandPrefabArray.Length);
        var islandPrefab = islandPrefabArray[random].Value;
        var cannonPosition = islandPrefabArray[random].CannonPosition;

        Entities.
            WithChangeFilter<IslandSpawnSettings>().
            ForEach((Entity e) =>
        {
            islandPrefabData.NextPrefab.Value = islandPrefab;
            islandPrefabData.NextPrefab.CannonPosition = cannonPosition;

            ecb.SetComponent(e, islandPrefabData);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
