using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CheckCollisionOnIslandSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

        _endFixedStepSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct CheckCollisionOnIslandSystemJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<GroundTag> GroundGroup;
        [ReadOnly] public ComponentDataFromEntity<CannonballTag> CannonballGroup;
        [ReadOnly] public ComponentDataFromEntity<CannonballOnGroundTag> CannonballOnGroundGroup;

        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            bool entityAIsCannonball = CannonballGroup.HasComponent(entityA);
            bool entityAIsGround = GroundGroup.HasComponent(entityA);
            bool entityAIsProcessed = CannonballOnGroundGroup.HasComponent(entityA);
            
            bool entityBIsCannonball = CannonballGroup.HasComponent(entityB);
            bool entityBIsGround = GroundGroup.HasComponent(entityB);
            bool entityBIsProcessed = CannonballOnGroundGroup.HasComponent(entityB);

            if (entityAIsCannonball && entityBIsGround && !entityAIsProcessed)
            {
                EntityCommandBuffer.AddComponent(entityA.Index, entityA, new CannonballOnGroundTag());
            }

            if (entityBIsCannonball && entityAIsGround && !entityBIsProcessed)
            {
                UnityEngine.Debug.Log("Entity A is ground and Entity B is cannonball");
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new CheckCollisionOnIslandSystemJob
        {
            GroundGroup = GetComponentDataFromEntity<GroundTag>(true),
            CannonballGroup = GetComponentDataFromEntity<CannonballTag>(true),
            CannonballOnGroundGroup = GetComponentDataFromEntity<CannonballOnGroundTag>(true),
            EntityCommandBuffer = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter()
        };
        
        var jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        
        _buildPhysicsWorld.AddInputDependencyToComplete(jobHandle);

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
