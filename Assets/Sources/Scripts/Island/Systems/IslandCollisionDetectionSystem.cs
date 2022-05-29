using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class IslandCollisionDetectionSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<CannonFiredTag>();
        RequireSingletonForUpdate<InGameTag>();

        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

        _endFixedStepSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    private struct IslandCollisionDetectionSystemJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<IslandTag> IslandGroup;
        [ReadOnly] public ComponentDataFromEntity<CannonballTag> CannonballGroup;
        [ReadOnly] public ComponentDataFromEntity<CannonballHitOnIslandTag> CannonballOnGroundGroup;
        [ReadOnly] public PhysicsWorld PhysicsWorld;

        public ComponentDataFromEntity<AnswerLabelData> CorrectAnswerLabelDataGroup;

        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
        public Entity GameManagerEntity;
        public Entity MenuManagerEntity;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            var entityAIsCannonball = CannonballGroup.HasComponent(entityA);
            var entityAIsProcessed = CannonballOnGroundGroup.HasComponent(GameManagerEntity);

            var entityBIsIsland = IslandGroup.HasComponent(entityB);

            if (!entityAIsCannonball || !entityBIsIsland || entityAIsProcessed) return;

            var answerLabel = CorrectAnswerLabelDataGroup[MenuManagerEntity];

            if (collisionEvent.CalculateDetails(ref PhysicsWorld).AverageContactPointPosition.y < 1f)
            {
                EntityCommandBuffer.AddComponent<CannonballMisshitTag>(GameManagerEntity.Index, GameManagerEntity);
                answerLabel.Type = AnswerType.Incorrect;
                
            }
            else
            {
                EntityCommandBuffer.AddComponent<CannonballHitOnIslandTag>(GameManagerEntity.Index, GameManagerEntity);
                answerLabel.Type = AnswerType.Correct;
            }

            EntityCommandBuffer.SetComponent(MenuManagerEntity.Index, MenuManagerEntity, answerLabel);
            EntityCommandBuffer.AddComponent(MenuManagerEntity.Index, MenuManagerEntity, new Timer(answerLabel.SecondsVisible));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new IslandCollisionDetectionSystemJob
        {
            IslandGroup = GetComponentDataFromEntity<IslandTag>(true),
            CannonballGroup = GetComponentDataFromEntity<CannonballTag>(true),
            CannonballOnGroundGroup = GetComponentDataFromEntity<CannonballHitOnIslandTag>(true),
            CorrectAnswerLabelDataGroup = GetComponentDataFromEntity<AnswerLabelData>(),
            EntityCommandBuffer = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter(),
            GameManagerEntity = GetSingletonEntity<GameManagerTag>(),
            MenuManagerEntity = GetSingletonEntity<MenuManagerTag>(),
            PhysicsWorld = _buildPhysicsWorld.PhysicsWorld
        };

        var jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);

        _buildPhysicsWorld.AddInputDependencyToComplete(jobHandle);

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
