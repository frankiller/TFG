using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public struct OperationAnswer : IComponentData
{
    public int Value;
    public float3 Position;
    public quaternion Rotation;
    public bool IsCorrect;
}

public class OperationSetterSystem : MonoBehaviour
{
    [SerializeField] private Text _operationText;
    [SerializeField] private int _optionsNumber;
    //[SerializeField] private Transform _centerTransform;
    [SerializeField] private float _radius = 5f;
    
    private Questions _questions;
    private int _instantiatedTargets = 0;

    private EntityManager _entityManager;

    protected  void Start()
    {
        _questions = ScriptableObject.CreateInstance<Questions>();
        _questions.optionsRange = _optionsNumber;

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        if (GameManager.IsGameOver() || !GameManager.IsPlayState()) { return; }

        if (_instantiatedTargets < _questions.optionsRange)
        {
            CreateOperation();
        }
    }

    private void CreateOperation()
    {
        var answer = _questions.SumOrMinusOperation();
        var answerList = _questions.RandomAnswerGenerator(answer);

        _operationText.text = _questions.operationText;

        InstantiateTarget(answer, answerList);
    }

    private void InstantiateTarget(int answer, IReadOnlyList<int> answerList)
    {
        var lookAtPosition = _entityManager.GetComponentData<Translation>(_entityManager.CreateEntityQuery(typeof(CannonTag)).ToEntityArray(Allocator.Temp)[0]);

        for (var i = 0; i < _questions.optionsRange; i++)
        {
            var randomPosition = Random.insideUnitCircle.normalized * _radius;
            var newPosition = new Vector3(randomPosition.x, _radius, randomPosition.y);
            var newRotation = Quaternion.LookRotation(new Vector3(lookAtPosition.Value.x, lookAtPosition.Value.y, lookAtPosition.Value.z) - newPosition);

            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new OperationAnswer
            {
                Position = newPosition,
                Rotation = newRotation,
                Value = answerList[i],
                IsCorrect = answerList[i] == answer
            });

            _instantiatedTargets++;
        }
    }
}
