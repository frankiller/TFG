using Unity.Entities;

public class StartSpawnInitialObjects : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<InitializeSystemsTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartSpawnInitialObjects").
            WithAll<InitializeSystemsTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<InitializeSystemsTag>(gameManagerEntity);
            ecb.AddComponent<SpawnInitialObjectsTag>(gameManagerEntity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartCreateInitialOperationsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<SpawnInitialObjectsTag>();
        RequireSingletonForUpdate<IslandTag>();
        RequireSingletonForUpdate<CannonTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartCreateInitialOperationsSystem").
            WithAll<SpawnInitialObjectsTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<SpawnInitialObjectsTag>(gameManagerEntity);
            ecb.AddComponent<CreateOperationsTag>(gameManagerEntity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartGetPlayerActionsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CreateOperationsTag>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new [] {ComponentType.ReadOnly<TargetTag>()},
            None = new [] {ComponentType.ReadOnly<TextMeshInternalData>()}
        }));
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartGetPlayerActionsSystem").
            WithAll<CreateOperationsTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<CreateOperationsTag>(gameManagerEntity);
                ecb.AddComponent<GetPlayerActionsTag>(gameManagerEntity);
            }).Schedule();
        
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartFireCannonSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GetPlayerActionsTag>();
        RequireSingletonForUpdate<CannonballTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartFireCannonSystem").
            WithAll<GetPlayerActionsTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<GetPlayerActionsTag>(gameManagerEntity);
            ecb.AddComponent<FireCannonTag>(gameManagerEntity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartUpdateObjectsPosition : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<FireCannonTag>();
        RequireSingletonForUpdate<CannonballHitOnIslandTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartUpdateObjectsPosition").
            WithAll<FireCannonTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<FireCannonTag>(gameManagerEntity);
            ecb.AddComponent<UpdateObjectsPositionTag>(gameManagerEntity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartCreateOperationsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
    private EntityQuery _targetQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _targetQuery = GetEntityQuery(ComponentType.ReadOnly<TargetTag>());

        RequireSingletonForUpdate<UpdateObjectsPositionTag>();
    }

    protected override void OnUpdate()
    {
        if (_targetQuery.CalculateEntityCount() != 0) return;

        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartCreateOperationsSystem").
            WithAll<UpdateObjectsPositionTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<UpdateObjectsPositionTag>(gameManagerEntity);
                ecb.AddComponent<CreateOperationsTag>(gameManagerEntity);
            }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
