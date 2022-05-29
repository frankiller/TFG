using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonShootSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endFixedStepSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("CannonShootSystem").
            ForEach((Entity entity, int entityInQueryIndex, ref PhysicsVelocity physicsVelocity, in CannonShootData cannonShootData) =>
            {
                physicsVelocity.Linear = math.normalize(cannonShootData.Direction) * cannonShootData.Force;

                ecb.RemoveComponent<CannonShootData>(entityInQueryIndex, entity);

            }).ScheduleParallel();

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
