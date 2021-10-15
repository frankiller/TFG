using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class IslandCreatorSystem : SystemBase
{
    private IslandSpawnerSystem _islandSpawnerSystem;
    private EntityQuery _positionPredictionDataQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _islandSpawnerSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<IslandSpawnerSystem>();
        _positionPredictionDataQuery = GetEntityQuery(typeof(PositionPredictionData));

        RequireForUpdate(_positionPredictionDataQuery);
    }

    protected override void OnUpdate()
    {
        var islandSpawnerEntity = GetSingletonEntity<GroundSpawnSettings>();
        var spawnData = GetComponent<Ground>(islandSpawnerEntity);

        using var entities = _positionPredictionDataQuery.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            var positionPredictionData = GetComponent<PositionPredictionData>(entity);

            EntityManager.SetComponentData(islandSpawnerEntity, new Ground
            {
                Position = positionPredictionData.PredictedPosition,
                Orientation = spawnData.Orientation,
                Size = spawnData.Size,
                BevelRadius = spawnData.BevelRadius,
                Center = spawnData.Center,
                Friction = spawnData.Friction,
                Restitution = spawnData.Restitution
            });

            _islandSpawnerSystem.Create(null);

            EntityManager.RemoveComponent<PositionPredictionData>(entity);
        }
    }
}
