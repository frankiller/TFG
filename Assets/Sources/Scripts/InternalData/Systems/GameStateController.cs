using Unity.Entities;

[UpdateAfter(typeof(GameStartSystem))]
public class StartLoadMenuSceneSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<LoadMenuSceneTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartLoadMenuSceneSystem").
            WithAll<LoadMenuSceneTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<LoadMenuSceneTag>(gameManagerEntity);
            }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartReloadMenuSceneSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<ReloadMenuSceneTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartReloadMenuSceneSystem").
            WithAll<ReloadMenuSceneTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<ReloadMenuSceneTag>(gameManagerEntity);
            }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class StartLoadGameSceneSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<LoadGameSceneTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartLoadGameSceneSystem").
            WithAll<LoadGameSceneTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<LoadGameSceneTag>(gameManagerEntity);
            }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

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

public class FinishCreateOperationsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CreateOperationsTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var gameManagerEntity = GetSingletonEntity<GameManagerTag>();
        var operationsBuffer = GetBuffer<OperationAnswerBuffer>(gameManagerEntity);

        if (operationsBuffer.Length > 0)
        {
            ecb.RemoveComponent<CreateOperationsTag>(gameManagerEntity);
            ecb.AddComponent<FinishedCreateOperationsTag>(gameManagerEntity);
        }

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public class FinishSpawnTargetsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<FinishedCreateOperationsTag>();
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
            WithName("FinishSpawnTargetsSystem").
            WithAll<FinishedCreateOperationsTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.AddComponent<FinishedSpawnTargetsTag>(gameManagerEntity);
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

        RequireSingletonForUpdate<FinishedCreateOperationsTag>();
        RequireSingletonForUpdate<FinishedSpawnTargetsTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartGetPlayerActionsSystem").
            WithAll<FinishedSpawnTargetsTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<FinishedCreateOperationsTag>(gameManagerEntity);
                ecb.RemoveComponent<FinishedSpawnTargetsTag>(gameManagerEntity);

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
