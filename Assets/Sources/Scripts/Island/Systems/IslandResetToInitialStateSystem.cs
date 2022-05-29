using Unity.Entities;
using Unity.Transforms;

public class IslandResetToInitialStateSystem : SystemBase
{
    private EntityManager _entityManager;
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<GameManagerTag>() },
            Any = new []
            {
                ComponentType.ReadOnly<ReloadMenuTag>(),
                ComponentType.ReadOnly<ReloadGameTag>()
            }
        }));

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _endSimulationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var islandSpawnerEntity = GetSingletonEntity<IslandSpawnerTag>();
        var islandPrefabBuffer = GetBuffer<IslandPrefabBuffer>(islandSpawnerEntity);
        var firstIslandPosition = _entityManager.GetComponentData<LocalToWorld>(islandPrefabBuffer[0].Value);
        var firstIslandPositionOffset = _entityManager.GetComponentData<IslandSpawnOffsetAuthoring>(islandSpawnerEntity);

        _entityManager.AddComponentData(islandSpawnerEntity,
            new CannonPositionData
            {
                Value = islandPrefabBuffer[0].CannonPosition + (firstIslandPosition.Position - firstIslandPositionOffset.Value)
            });

        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("IslandResetToInitialStateSystem").
            WithAll<IslandTag>().
            WithNone<IsFirstIslandTag>().
            ForEach((Entity islandEntity, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, islandEntity)).
            ScheduleParallel();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
