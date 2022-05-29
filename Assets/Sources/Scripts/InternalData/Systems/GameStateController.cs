using Unity.Entities;

[UpdateAfter(typeof(GameStartSystem))]
public class StartLoadMenuSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<LoadMenuTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartLoadMenuSystem").
            WithAll<LoadMenuTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<LoadMenuTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartLoadMenuSystem");
    }
}

public class StartReloadMenuSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<ReloadMenuTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartReloadMenuSystem").
            WithAll<ReloadMenuTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<ReloadMenuTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartReloadMenuSystem");
    }
}

public class StartLoadGameSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<LoadGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartLoadGameSystem").
            WithAll<LoadGameTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<LoadGameTag>(gameManagerEntity);
                ecb.AddComponent<InGameTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartLoadGameSystem");
    }
}

public class StartReloadGameSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<ReloadGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartReloadGameSystem").
            WithAll<ReloadGameTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<ReloadGameTag>(gameManagerEntity);
                ecb.AddComponent<LoadGameTag>(gameManagerEntity);
                ecb.AddComponent<InitializeGameSystemsTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartReloadGameSystem");
    }
}

public class StartSpawnInitialObjects : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<InitializeGameSystemsTag>();
        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartSpawnInitialObjects").
            WithAll<InitializeGameSystemsTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<InitializeGameSystemsTag>(gameManagerEntity);
            ecb.AddComponent<SpawnInitialObjectsTag>(gameManagerEntity);
        }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartSpawnInitialObjects");
    }
}

public class StartCreateInitialOperationsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<SpawnInitialObjectsTag>();
        RequireSingletonForUpdate<IslandTag>();
        RequireSingletonForUpdate<CannonTag>();
        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartCreateInitialOperationsSystem").
            WithAll<SpawnInitialObjectsTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<SpawnInitialObjectsTag>(gameManagerEntity);
            ecb.AddComponent<CreateOperationsTag>(gameManagerEntity);
        }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartCreateInitialOperationsSystem");
    }
}

public class FinishCreateOperationsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CreateOperationsTag>();
        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();
        var gameManagerEntity = GetSingletonEntity<GameManagerTag>();
        var operationsBuffer = GetBuffer<OperationAnswerBuffer>(gameManagerEntity);

        if (operationsBuffer.Length > 0)
        {
            ecb.RemoveComponent<CreateOperationsTag>(gameManagerEntity);
            ecb.AddComponent<FinishedCreateOperationsTag>(gameManagerEntity);
        }

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("FinishCreateOperationsSystem");
    }
}

public class FinishSpawnTargetsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<FinishedCreateOperationsTag>();
        RequireSingletonForUpdate<InGameTag>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TargetTag>()
            },
            None = new[] { ComponentType.ReadOnly<TextMeshInternalData>() }
        }));
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("FinishSpawnTargetsSystem").
            WithAll<FinishedCreateOperationsTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.AddComponent<FinishedSpawnTargetsTag>(gameManagerEntity);
                ecb.AddComponent<UpdateLabelTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("FinishSpawnTargetsSystem");
    }
}

public class StartGetPlayerActionsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<FinishedCreateOperationsTag>();
        RequireSingletonForUpdate<FinishedSpawnTargetsTag>();
        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartGetPlayerActionsSystem").
            WithAll<FinishedSpawnTargetsTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<FinishedCreateOperationsTag>(gameManagerEntity);
                ecb.RemoveComponent<FinishedSpawnTargetsTag>(gameManagerEntity);

                ecb.AddComponent<GetPlayerActionsTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartGetPlayerActionsSystem");
    }
}

public class StartFireCannonSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GetPlayerActionsTag>();
        RequireSingletonForUpdate<InGameTag>();
        RequireSingletonForUpdate<CannonballTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartFireCannonSystem").
            WithAll<GetPlayerActionsTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<GetPlayerActionsTag>(gameManagerEntity);
            ecb.AddComponent<CannonFiredTag>(gameManagerEntity);
            ecb.AddComponent<UpdateLabelTag>(gameManagerEntity);
        }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartFireCannonSystem");
    }
}

public class StartUpdateUi : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<GameManagerTag>() },
            Any = new[]
            {
                ComponentType.ReadOnly<CannonballHitOnIslandTag>(),
                ComponentType.ReadOnly<CannonballMisshitTag>()
            }
        }));

        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartUpdateUi").
            WithAll<CannonFiredTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<CannonFiredTag>(gameManagerEntity);
                ecb.AddComponent<UpdateUiTag>(gameManagerEntity);
                ecb.AddComponent<UpdateLabelTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartUpdateUi");
    }
}

public class StartUpdateObjectsPosition : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<UpdateUiTag>();
        RequireSingletonForUpdate<InGameTag>();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<MenuManagerTag>() },
            None = new[]
            {
                ComponentType.ReadOnly<UpdateLabelTag>(),
                ComponentType.ReadOnly<Timer>()
            }
        }));
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartUpdateObjectsPosition").
            WithAll<UpdateUiTag>().
            ForEach((Entity gameManagerEntity) =>
        {
            ecb.RemoveComponent<UpdateUiTag>(gameManagerEntity);
            ecb.AddComponent<UpdateObjectsPositionTag>(gameManagerEntity);
        }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartUpdateObjectsPosition");
    }
}

public class StartCreateOperationsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
    private EntityQuery _targetQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _targetQuery = GetEntityQuery(ComponentType.ReadOnly<TargetTag>());

        RequireSingletonForUpdate<UpdateObjectsPositionTag>();
        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        if (!_targetQuery.IsEmpty) return;

        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("StartCreateOperationsSystem").
            WithAll<UpdateObjectsPositionTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<UpdateObjectsPositionTag>(gameManagerEntity);
                ecb.AddComponent<CreateOperationsTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("StartCreateOperationsSystem");
    }
}

public class FinishGameCycleSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<UpdateObjectsPositionTag>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<GameManagerTag>() },
            None = new[] { ComponentType.ReadOnly<InGameTag>() }
        }));
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithName("FinishGameCycleSystem").
            WithAll<UpdateObjectsPositionTag>().
            ForEach((Entity gameManagerEntity) =>
            {
                ecb.RemoveComponent<UpdateObjectsPositionTag>(gameManagerEntity);
            }).Schedule();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        //UnityEngine.Debug.Log("FinishGameCycleSystem");
    }
}
