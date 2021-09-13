using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonMoverSystem : SystemBase
{
    private EntityQuery _cannonEntityQuery;
    private EntityManager _entityManager;

    private float2 _turnSpeed;
    private float2 _acceleration;
    private float _maxVerticalAngle;
    private float _inputLagPeriod;

    private float _inputLagTimer;
    
    private float _lastXInputEvent;
    private float _lastYInputEvent;

    private float _currentYVelocity;
    private float _currentYRotation;

    private float _currentZVelocity;
    private float _currentZRotation;
    private float _currentZEulerRotation;

    private float GetXInput
    {
        get
        {
            _inputLagTimer += Time.DeltaTime;

            var input = Input.GetAxis("Mouse X");

            if (Mathf.Approximately(0, input) && !(_inputLagTimer >= _inputLagPeriod)) return _lastXInputEvent;
            _lastXInputEvent = input;
            _inputLagTimer = 0f;

            return _lastXInputEvent;
        }
    }

    private float GetYInput
    {
        get
        {
            _inputLagTimer += Time.DeltaTime;

            var input = Input.GetAxis("Mouse Y");

            if (Mathf.Approximately(0, input) && !(_inputLagTimer >= _inputLagPeriod)) return _lastYInputEvent;
            _lastYInputEvent = input;
            _inputLagTimer = 0f;

            return _lastYInputEvent;
        }
    }


    protected override void OnStartRunning()
    {
        _cannonEntityQuery = GetEntityQuery(typeof(CannonTag));
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var inputVariables = GetComponent<InputVariables>(GetSingletonEntity<InputVariables>());
        _turnSpeed = inputVariables.TurnSpeed;
        _acceleration = inputVariables.Acceleration;
        _maxVerticalAngle = inputVariables.MaxVerticalAngle;
        _inputLagPeriod = inputVariables.InputLagPeriod;

        _currentYVelocity = 0f;
        _currentZVelocity = 0f;
        
        _lastXInputEvent = 0f;
        _lastYInputEvent = 0f;

        _inputLagTimer = 0f;

        _currentZEulerRotation = CannonManager.GetCannonBarrelRotation().value.z;

        if (_currentZEulerRotation >= 180)
        {
            _currentZEulerRotation -= 360;
        }

        _currentZEulerRotation = ClampVerticalAngle(_currentZEulerRotation);

        _currentYRotation = CannonManager.GetCannonBarrelRotation().value.y;
        _currentZRotation = _currentZEulerRotation;
    }

    protected override void OnUpdate()
    {
        var entity = _cannonEntityQuery.GetSingletonEntity();
        var linkedEntityGroup = GetBuffer<LinkedEntityGroup>(entity);
        
        var baseEntity = linkedEntityGroup[1].Value;
        _entityManager.SetComponentData(baseEntity, new Rotation{ Value = GetYRotationToMouse() * Quaternion.Euler(-90f, 0f, 0f)});
        
        var barrelEntity = linkedEntityGroup[2].Value;
        _entityManager.SetComponentData(barrelEntity, new Rotation { Value = GetYRotationToMouse() * Quaternion.Euler(0f, 0f, 90f) * GetZRotationToMouse()});
    }

    private Quaternion GetYRotationToMouse()
    {
        var wantedVelocity = GetXInput * _turnSpeed.x;
        _currentYVelocity = Mathf.MoveTowards(_currentYVelocity, wantedVelocity, _acceleration.x * Time.DeltaTime);

        _currentYRotation += _currentYVelocity * Time.DeltaTime;

        return Quaternion.Euler(0f, _currentYRotation, 0f);
    }

    private Quaternion GetZRotationToMouse()
    {
        var wantedVelocity = GetYInput * _turnSpeed.y;
        _currentZVelocity = Mathf.MoveTowards(_currentZVelocity, wantedVelocity, _acceleration.y * Time.DeltaTime);

        _currentZRotation += _currentZVelocity * Time.DeltaTime;
        _currentZRotation = ClampVerticalAngle(_currentZRotation);

        return Quaternion.Euler(0f, 0f, -_currentZRotation);
    }

    private float ClampVerticalAngle(float angle)
    {
        return math.clamp(angle, -_maxVerticalAngle, _maxVerticalAngle);
    }
}