using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonRepositionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CannonPositionData>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var cannonEntity = GetSingletonEntity<CannonTag>();

        Entities.
            WithName("CannonRepositionSystem").
            WithChangeFilter<CannonPositionData>().
            ForEach((Entity entity, in CannonPositionData cannonPosition) =>
        {
            ecb.SetComponent(cannonEntity, new Translation { Value = cannonPosition.Value + new float3(0f, .5f, 0f) });
            ecb.RemoveComponent<CannonPositionData>(entity);
        }).Schedule();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
