using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonRepositionSystem : SystemBase
{
    private EntityManager _entityManager;
    private EntityQuery _cannonEntityQuery;
    private EntityQuery _cameraEntityQuery;
    private EntityQuery _cannonMuzzleEntityQuery;
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _cannonEntityQuery = _entityManager.CreateEntityQuery(typeof(CannonTag));
        _cameraEntityQuery = _entityManager.CreateEntityQuery(typeof(CameraTag));
        _cannonMuzzleEntityQuery = _entityManager.CreateEntityQuery(typeof(CannonMuzzleTag));
        _endFixedStepSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireForUpdate(_cannonEntityQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter();
        var camera = _cameraEntityQuery.GetSingletonEntity();
        var cannonMuzzleEntity = _cannonMuzzleEntityQuery.GetSingletonEntity();
        var cannonEntity = _cannonEntityQuery.GetSingletonEntity();
        var islandSpawnerEntity = GetSingletonEntity<GroundSpawnSettings>();
        var spawnData = GetComponent<Ground>(islandSpawnerEntity);

        Entities.
            WithName("CannonRepositionSystem").
            WithAny<CannonballOnGroundTag>().
            ForEach((Entity cannonballEntity, int entityInQueryIndex) =>
            {
                ecb.SetComponent(cannonEntity.Index, cannonEntity, new Translation { Value = spawnData.Position + new float3(0f, .5f, 0f)});
                ecb.SetComponent(camera.Index, camera, new CameraTargetEntityData { Value = cannonMuzzleEntity });
            }).ScheduleParallel();

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(CannonRepositionSystem))]
[UpdateAfter(typeof(CheckCollisionOnIslandSystem))]
public class OperationTargetRepositionSystem : SystemBase
{
    private EntityManager _entityManager;
    private EntityQuery _operationTargetQuery;
    private EntityQuery _cannonballOnGroundQuery;
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;
        _operationTargetQuery = _entityManager.CreateEntityQuery(typeof(TargetTag));
        _cannonballOnGroundQuery = _entityManager.CreateEntityQuery(typeof(CannonballOnGroundTag));
        _endFixedStepSimulationEntityCommandBuffer = currentWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireForUpdate(_cannonballOnGroundQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer().AsParallelWriter();

        var job1 = Entities.
            WithName("OperationTargetRepositionSystem").
            WithAll<TargetTag>().
            ForEach((Entity targetEntity, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, targetEntity)).
            ScheduleParallel(Dependency);
        //Debería ir en un sistema aparte para gestionar la bola de cañón
        var job2 = Entities.
            WithAll<CannonballOnGroundTag>().
            ForEach((Entity cannonballOnGround, int entityInQueryIndex) => ecb.DestroyEntity(entityInQueryIndex, cannonballOnGround)).
            ScheduleParallel(job1);

        var gameManagerEntity = _entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
        var gameInternalData = _entityManager.GetComponentData<OperationsInternalDataAuthoring>(gameManagerEntity);
        gameInternalData.CreateOperations = true;
        _entityManager.SetComponentData(gameManagerEntity, gameInternalData);

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(job2);

        GameManager.StartPlayState();
    }
}
