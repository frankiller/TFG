using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
public class EndGameEntityDestroySystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endFixedStepSimulationEntityCommandBuffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!GameManager.IsGameOver()) { return; }

        var ecs = _endFixedStepSimulationEntityCommandBuffer
            .CreateCommandBuffer()
            .AsParallelWriter();

        Entities
            .WithName("EntityDestroy")
            .WithAny<TargetPrefabConversion, TargetTag, PhysicsCollider>()
            .ForEach((Entity targetPrefab, int entityInQueryIndex) => ecs.DestroyEntity(entityInQueryIndex, targetPrefab)).ScheduleParallel();

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
