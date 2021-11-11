using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(CannonRepositionSystem))]
[UpdateAfter(typeof(IslandCollisionDetectionSystem))]
public class TargetRepositionSystem : SystemBase
{
    private EntityManager _entityManager;
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;
        _endFixedStepSimulationEntityCommandBuffer = currentWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<UpdateObjectsPositionTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter();

        var job1 = Entities.
            WithName("TargetRepositionSystem").
            WithAll<TargetTag>().
            ForEach((Entity targetEntity, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, targetEntity)).
            ScheduleParallel(Dependency);
        //Debería ir en un sistema aparte para gestionar la bola de cañón
        var job2 = Entities.
            WithAll<CannonballTag>().
            ForEach((Entity cannonballOnGround, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, cannonballOnGround)).
            ScheduleParallel(job1);

        var gameInternalData = GetSingleton<OperationsInternalDataAuthoring>();
        gameInternalData.CreateOperations = true;
        _entityManager.SetComponentData(GetSingletonEntity<GameManagerTag>(), gameInternalData);

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(job2);
    }
}