using Unity.Entities;

public class TargetResetToInitialStateSystem : SystemBase
{
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

        _endSimulationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithAll<TargetTag>().
            ForEach((Entity targetEntity, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, targetEntity)).
            ScheduleParallel();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
