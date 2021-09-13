using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
public class TargetSpawnerSystem : SystemBase
{
    private EntityManager _entityManager;
    private EntityQuery _operationAnswerEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;

        _operationAnswerEntityQuery = _entityManager.CreateEntityQuery(new ComponentType[] {typeof(OperationAnswer)});

        RequireForUpdate(_operationAnswerEntityQuery);
    }

    protected override void OnUpdate()
    {
        if (!GameManager.IsPlayState()) { return;  }

        var targetPrefabEntity = GetSingletonEntity<TargetPrefabConversion>();
        var targetPrefab = GetComponent<TargetPrefabConversion>(targetPrefabEntity).TargetPrefab;

        using var entities = _operationAnswerEntityQuery.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            var operationAnswer = GetComponent<OperationAnswer>(entity);
            var newTarget = _entityManager.Instantiate(targetPrefab);

            _entityManager.SetComponentData(newTarget, new Translation { Value = operationAnswer.Position });
            _entityManager.SetComponentData(newTarget, new Rotation { Value = operationAnswer.Rotation});
            _entityManager.GetComponentObject<TextMesh>(_entityManager.GetBuffer<LinkedEntityGroup>(newTarget)[3].Value).text = $"{operationAnswer.Value}";
            _entityManager.AddComponentData(newTarget, new TargetTag());

            if (operationAnswer.IsCorrect)
            {
                _entityManager.AddComponentData(newTarget, new IsCorrectTag());
            }

            _entityManager.RemoveComponent<OperationAnswer>(entity);
        }
    }

    protected override void OnDestroy()
    {
        _entityManager.RemoveComponent<TargetPrefabConversion>(_entityManager
            .CreateEntityQuery(typeof(TargetPrefabConversion))
            .ToEntityArray(Allocator.Temp));
    }
}
