using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
public class EndGameEntityDestroySystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endFixedStepSimulationEntityCommandBuffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<DeactivateSystemsTag>();
    }

    protected override void OnUpdate()
    {
        var ecs = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("EntityDestroy")
            .WithEntityQueryOptions(EntityQueryOptions.IncludePrefab)
            .ForEach((Entity targetPrefab, int entityInQueryIndex) => ecs.DestroyEntity(entityInQueryIndex, targetPrefab)).ScheduleParallel();

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
