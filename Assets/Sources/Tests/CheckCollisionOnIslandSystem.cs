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

    protected override void OnCreate()
    {
        base.OnCreate();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    [BurstCompile]
    struct CheckCollisionOnIslandSystemJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<GroundTag> GroundGroup;
        public ComponentDataFromEntity<CannonballTag> CannonballGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            bool entityAIsCannonball = CannonballGroup.HasComponent(entityA);
            bool entityAIsGround = GroundGroup.HasComponent(entityA);
            bool entityBIsCannonball = CannonballGroup.HasComponent(entityB);
            bool entityBIsGround = GroundGroup.HasComponent(entityB);

            if (entityAIsCannonball && entityBIsGround)
            {
                UnityEngine.Debug.Log("Entity A is cannonball and Entity B is ground");
            }

            if (entityBIsCannonball && entityAIsGround)
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
            CannonballGroup = GetComponentDataFromEntity<CannonballTag>()
        };

        var jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
