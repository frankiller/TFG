using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
[UpdateAfter(typeof(OperationCreatorSystem))]
public class TargetSpawnSystem : SystemBase
{
    private EntityManager _entityManager;
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;
    private Entity _targetPrefab;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;
        _endFixedStepSimulationEntityCommandBuffer = currentWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<FinishedCreateOperationsTag>();
        RequireSingletonForUpdate<InGameTag>();
        RequireSingletonForUpdate<TargetPrefabConversion>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        _targetPrefab = GetSingleton<TargetPrefabConversion>().TargetPrefab;
    }

    protected override void OnUpdate()
    {
        var targetPrefab = _targetPrefab; //Es una guarrada, quizás con BlobAssets mejore la sintaxis
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer();

        Entities.
            WithName("TargetSpawnSystem").
            ForEach((ref DynamicBuffer<OperationAnswerBuffer> operations) =>
            {
                for (int i = 0; i < operations.Length; i++)
                {
                    var newTarget = ecb.Instantiate(targetPrefab);

                    ecb.AddComponent(newTarget, new TargetTag());
                    ecb.SetComponent(newTarget, new Translation { Value = operations[i].Position });
                    ecb.SetComponent(newTarget, new Rotation { Value = operations[i].Rotation });

                    ecb.AddComponent(newTarget, new TextMeshInternalData
                    {
                        TextValue = operations[i].Value
                    });

                    if (operations[i].IsCorrect)
                    {
                        ecb.AddComponent(newTarget, new IsCorrectTag());
                    }
                }

                operations.Clear();
            }).Schedule();

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
