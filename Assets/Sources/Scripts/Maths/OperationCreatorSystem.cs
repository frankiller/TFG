using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class OperationCreatorSystem : SystemBase
{
    private EntityManager _entityManager;
    private Questions _questions;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        RequireSingletonForUpdate<CreateOperationsTag>();
        RequireSingletonForUpdate<UiOperationTextData>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        _questions = new Questions {OptionsRange = GetSingleton<OperationsInternalDataAuthoring>().OperationRange};
    }

    protected override void OnUpdate()
    {
        CreateOperations();
    }

    private void CreateOperations()
    {
        var gameStartData = GetSingleton<GameStartData>();
        var answer = gameStartData.Mode switch
        {
            GameMode.SumSubstract => _questions.SumOrMinusOperation(),
            GameMode.MultiplyDivide => _questions.MultiplyOperation(),
            GameMode.Multioperation => _questions.MultiOperandOperation(),
            GameMode.Algebraic => _questions.AlgebraicOperation(),
            _ => 0,
        };
        var answerList = _questions.RandomAnswerGenerator(answer);

        var operationTextData = _entityManager.GetComponentObject<UiOperationTextData>(GetSingletonEntity<UiOperationTextData>());
        operationTextData.Value.text = _questions.OperationText;

        FillOperationBuffer(answer, answerList, operationTextData.Radius);
    }

    private void FillOperationBuffer(int answer, IReadOnlyList<int> answerList, float radius)
    {
        var lookAtPosition = GetComponent<Translation>(GetSingletonEntity<CannonTag>());
        var buffer = GetBuffer<OperationAnswerBuffer>(GetSingletonEntity<GameManagerTag>());

        for (var i = 0; i < _questions.OptionsRange; i++)
        {
            var randomPosition = Random.insideUnitCircle.normalized * radius + new Vector2(lookAtPosition.Value.x, lookAtPosition.Value.z);
            var newPosition = new Vector3(randomPosition.x, lookAtPosition.Value.y + radius, randomPosition.y);
            var newRotation = Quaternion.LookRotation(new Vector3(lookAtPosition.Value.x, lookAtPosition.Value.y, lookAtPosition.Value.z) - newPosition);

            buffer.Add(new OperationAnswerBuffer
            {
                Position = newPosition,
                Rotation = newRotation,
                Value = answerList[i],
                IsCorrect = answerList[i] == answer
            });
        }
    }
}

public class DifficultyUpdaterSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<InitializeSystemsTag>();
    }

    protected override void OnUpdate()
    {
        var gameStartData = GetSingleton<GameStartData>();
        var operationRange = gameStartData.Difficulty switch
        {
            GameDifficulty.Test => 1,
            GameDifficulty.Easy => 3,
            GameDifficulty.Intermediate => 6,
            GameDifficulty.Hard => 9,
            _ => 1,
        };

        World.DefaultGameObjectInjectionWorld.EntityManager.
            AddComponentData(GetSingletonEntity<GameManagerTag>(), new OperationsInternalDataAuthoring {OperationRange = operationRange});
    }
}