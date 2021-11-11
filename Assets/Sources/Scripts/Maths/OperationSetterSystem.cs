using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OperationSetterSystem : MonoBehaviour
{
    [SerializeField] private Text operationText; //Podr�a ir en un componente authoring de tipo clase y que se actualice en un sistema aparte
    [SerializeField] private float radius = 5f;
    
    private Questions _questions;

    private EntityManager _entityManager;

    protected void Start()
    {
        _questions = ScriptableObject.CreateInstance<Questions>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        if (GameManager.IsGameOver() || !GameManager.IsPlayState()) { return; }

        var gameManagerEntity = _entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
        var gameInternalData = _entityManager.GetComponentData<OperationsInternalDataAuthoring>(gameManagerEntity);

        if (!gameInternalData.CreateOperations) return;

        _questions.optionsRange = gameInternalData.OperationRange;
        CreateOperations();

        gameInternalData.CreateOperations = false;
        _entityManager.SetComponentData(gameManagerEntity, gameInternalData);
    }

    private void CreateOperations()
    {
        var answer = _questions.SumOrMinusOperation();
        var answerList = _questions.RandomAnswerGenerator(answer);

        operationText.text = _questions.operationText;

        FillOperationBuffer(answer, answerList);
    }

    private void FillOperationBuffer(int answer, IReadOnlyList<int> answerList)
    {
        var lookAtPosition = _entityManager.GetComponentData<Translation>(_entityManager.CreateEntityQuery(typeof(CannonTag)).GetSingletonEntity());
        var buffer = _entityManager.GetBuffer<OperationAnswerBuffer>(_entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity());

        for (var i = 0; i < _questions.optionsRange; i++)
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
