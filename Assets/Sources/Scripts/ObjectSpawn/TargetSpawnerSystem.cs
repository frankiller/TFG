using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public struct TextMeshInternalData : IComponentData
{
    public int TextValue;
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
public class TargetSpawnerSystem : SystemBase
{
    private EntityManager _entityManager;
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;
        _endFixedStepSimulationEntityCommandBuffer = currentWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!GameManager.IsPlayState()) { return; }

        var targetPrefab = GetComponent<TargetPrefabConversion>(GetSingletonEntity<TargetPrefabConversion>()).TargetPrefab;
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer();

        Entities.
            WithName("TargetSpawnerSystem").
            ForEach((ref DynamicBuffer<OperationAnswer> operations) =>
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

    protected override void OnDestroy()
    {
        _entityManager.RemoveComponent<TargetPrefabConversion>(_entityManager
            .CreateEntityQuery(typeof(TargetPrefabConversion))
            .ToEntityArray(Allocator.Temp));
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
[UpdateAfter(typeof(TargetSpawnerSystem))]
public class UpdateTargetDataSystem : SystemBase
{
    private EntityQuery _targetQuery;
    private EntityManager _entityManager;
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;
        _targetQuery = _entityManager.CreateEntityQuery(typeof(TargetTag), typeof(TextMeshInternalData));
        _endFixedStepSimulationEntityCommandBuffer = currentWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireForUpdate(_targetQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBuffer.CreateCommandBuffer();

        using var targetEntities = _targetQuery.ToEntityArray(Allocator.Temp);
        foreach (var targetEntity in targetEntities)
        {
            _entityManager.GetComponentObject<TextMesh>(GetBuffer<LinkedEntityGroup>(targetEntity)[3].Value).text =
                $"{GetComponent<TextMeshInternalData>(targetEntity).TextValue}";

            ecb.RemoveComponent<TextMeshInternalData>(targetEntity);
        }

        _endFixedStepSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
