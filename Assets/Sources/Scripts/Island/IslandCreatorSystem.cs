using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class IslandCreatorSystem : SystemBase
{
    private IslandSpawnerSystem _islandSpawnerSystem;
    private EntityQuery _cannonShootDataQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _islandSpawnerSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<IslandSpawnerSystem>();
        _cannonShootDataQuery = GetEntityQuery(new ComponentType [] { typeof(CannonShootData) , typeof(IsCorrect)});
    }

    protected override void OnUpdate()
    {
        if (_cannonShootDataQuery.CalculateEntityCount() == 0) {return;}

        var groundSpawnSettingsEntity = GetSingletonEntity<GroundSpawnSettings>();
        var spawnData = GetComponent<Ground>(groundSpawnSettingsEntity);

        using var entities = _cannonShootDataQuery.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            var cannonShootData = EntityManager.GetComponentData<CannonShootData>(entity);

            EntityManager.AddComponentData(groundSpawnSettingsEntity, new Ground
            {
                Position = cannonShootData.PredictedPosition,
                Orientation = spawnData.Orientation,
                Size = spawnData.Size,
                BevelRadius = spawnData.BevelRadius,
                Center = spawnData.Center,
                Friction = spawnData.Friction,
                Restitution = spawnData.Restitution
            });

            _islandSpawnerSystem.Create(null);
        }
    }
}
