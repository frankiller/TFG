using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(CannonRepositionSystem))]
[UpdateAfter(typeof(IslandCollisionDetectionSystem))]
public class TargetDestroySystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endFixedStepSimulationEntityCommandBuffer = World.DefaultGameObjectInjectionWorld.
            GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<UpdateObjectsPositionTag>();
        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("TargetDestroySystem").
            WithAll<TargetTag>().
            ForEach((Entity targetEntity, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, targetEntity)).
            ScheduleParallel();

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}