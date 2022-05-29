using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(TargetSpawnSystem))]
public class UpdateTargetTextMeshSystem : SystemBase
{
    private EntityQuery _targetQuery;
    private EntityManager _entityManager;
    private EndFixedStepSimulationEntityCommandBufferSystem _entityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;

        _entityCommandBuffer = currentWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        _targetQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TargetTag>(),
                ComponentType.ReadWrite<TextMeshInternalData>()
            }
        });

        RequireForUpdate(_targetQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBuffer.CreateCommandBuffer();

        foreach (var targetEntity in _targetQuery.ToEntityArray(Allocator.Temp))
        {
            var textMeshEntity = GetBuffer<LinkedEntityGroup>(targetEntity)[3].Value;
            var textValue = GetComponent<TextMeshInternalData>(targetEntity).TextValue;

            var gameStartData = GetSingleton<GameStartData>();
            if (gameStartData.Mode == GameMode.FractionSumOrSubstract || gameStartData.Mode == GameMode.FractionMultiplyOrDivide)
            {
                _entityManager.GetComponentObject<TextMesh>(textMeshEntity).text = $"{new Fraction(textValue).Definition}";
            }
            else
            {
                _entityManager.GetComponentObject<TextMesh>(textMeshEntity).text = $"{textValue}";
            }

            ecb.RemoveComponent<TextMeshInternalData>(targetEntity);
        }

        _entityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
