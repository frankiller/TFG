using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class OperationCreatorSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<CreateOperationsTag>();
        RequireSingletonForUpdate<InGameTag>();
        RequireSingletonForUpdate<UiOperationTextData>();
    }

    protected override void OnUpdate()
    {
        BuildOperationAnswer(out var operationAnswer);

        SetOperationTextDefinition(operationAnswer);

        FillOperationBuffer(operationAnswer);
    }

    private void BuildOperationAnswer(out Operation operationAnswer)
    {
        var gameStartData = GetSingleton<GameStartData>();
        operationAnswer = gameStartData.Mode switch
        {
            GameMode.SumOrSubstract => OperationBuilder.SumOrMinus(),
            GameMode.MultiplyOrDivide => OperationBuilder.MultiplyOrDivide(),
            GameMode.Multioperation => OperationBuilder.MultiOperand(),
            GameMode.Algebraic => OperationBuilder.AlgebraicOperation(),
            GameMode.FractionSumOrSubstract => OperationBuilder.FractionSumOrMinus(),
            GameMode.FractionMultiplyOrDivide => OperationBuilder.FractionMultiplyOrDivide(),
            _ => new Operation { Solution = 0f }
        };
    }

    private void SetOperationTextDefinition(Operation operation)
    {
        var operationTextData = EntityManager.GetComponentObject<UiOperationTextData>(GetSingletonEntity<MenuManagerTag>());
        operationTextData.Value.text = operation.Definition;
    }

    private void FillOperationBuffer(Operation operationAnswer)
    {
        var optionsRange = GetSingleton<OperationRangeData>().Value;
        var operationList = RandomizationHelper.GenerateOperationList(operationAnswer, optionsRange);
        var angleSegment = 2 * Mathf.PI / (optionsRange);
        var lookAtPosition = GetComponent<Translation>(GetSingletonEntity<CannonTag>());
        var buffer = GetBuffer<OperationAnswerBuffer>(GetSingletonEntity<GameManagerTag>());

        for (var i = 0; i < optionsRange; i++)
        {
            var angle = angleSegment * i;
            var randomPosition = new Vector2(lookAtPosition.Value.x, lookAtPosition.Value.z) +
                                 new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * GetSingleton<TargetRadiusData>().Value;

            var height = lookAtPosition.Value.y + 50f + (i + 1) % 2 * 40f;
            var newPosition = new Vector3(randomPosition.x, height, randomPosition.y);
            var newRotation = Quaternion.LookRotation(new Vector3(lookAtPosition.Value.x, lookAtPosition.Value.y, lookAtPosition.Value.z) - newPosition);

            buffer.Add(new OperationAnswerBuffer
            {
                Position = newPosition,
                Rotation = newRotation,
                Value = operationList[i].Solution,
                IsCorrect = operationList[i] == operationAnswer
            });
        }
    }
}