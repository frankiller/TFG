using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[Serializable]
public struct CannonShootData : IComponentData
{
    public float3 Direction;
    public float Force;
    public float3 PredictedPosition;
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonShootSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;
    

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("CannonShootSystem").
            ForEach(
                (Entity entity, int entityInQueryIndex, ref PhysicsVelocity physicsVelocity, in CannonShootData cannonShootData) =>
                {
                    physicsVelocity.Linear = math.normalize(cannonShootData.Direction) * cannonShootData.Force;

                    ecb.RemoveComponent<CannonShootData>(entityInQueryIndex, entity);
                    
                }).ScheduleParallel();

        _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}
