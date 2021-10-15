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

    private float _turnSpeed;
    private float _acceleration;
    private float _maxVerticalAngle;
    private float _inputLagPeriod;

    private float _inputLagTimer;
    
    private float _lastXInputEvent;
    private float _lastYInputEvent;

    private float _currentYVelocity;
    private float _currentYRotation;

    private float _currentXVelocity;
    private float _currentXRotation;
    private float _currentXEulerRotation;

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
        _currentXVelocity = 0f;
        
        _lastXInputEvent = 0f;
        _lastYInputEvent = 0f;

        _inputLagTimer = 0f;

        var barrelRotation = CannonManager.GetCannonBarrelRotation(_entityManager).value;
        _currentXEulerRotation = new Quaternion(barrelRotation.x, barrelRotation.y, barrelRotation.z, barrelRotation.w).eulerAngles.x;

        if (_currentXEulerRotation >= 180)
        {
            _currentXEulerRotation -= 360;
        }

        _currentXEulerRotation = ClampVerticalAngle(_currentXEulerRotation);

        _currentYRotation = CannonManager.GetCannonBarrelRotation(_entityManager).value.y;
        _currentXRotation = _currentXEulerRotation;
    }

    protected override void OnUpdate()
    {
        var entity = _cannonEntityQuery.GetSingletonEntity();
        var linkedEntityGroup = GetBuffer<LinkedEntityGroup>(entity);
        var yRotation = GetYRotationToMouse();
        
        var baseEntity = linkedEntityGroup[1].Value;
        var fromRotation = _entityManager.GetComponentData<Rotation>(baseEntity).Value;
        var toRotation = quaternion.Euler(0f, yRotation, 0f);
        _entityManager.SetComponentData(baseEntity, new Rotation { Value = Quaternion.Lerp(fromRotation, toRotation, Time.DeltaTime * 5f)});

        var barrelEntity = linkedEntityGroup[2].Value;
        fromRotation = _entityManager.GetComponentData<Rotation>(barrelEntity).Value;
        toRotation = quaternion.Euler(GetXRotationToMouse(), yRotation, 0f);
        _entityManager.SetComponentData(barrelEntity, new Rotation { Value = Quaternion.Lerp(fromRotation, toRotation, Time.DeltaTime * 5f)});
    }

    private float GetYRotationToMouse()
    {
        var wantedVelocity = GetXInput * _turnSpeed;
        _currentYVelocity = Mathf.MoveTowards(_currentYVelocity, wantedVelocity, _acceleration * Time.DeltaTime);

        _currentYRotation += _currentYVelocity * Time.DeltaTime;

        return _currentYRotation;
    }

    private float GetXRotationToMouse()
    {
        var wantedVelocity = GetYInput * _turnSpeed;
        _currentXVelocity = Mathf.MoveTowards(_currentXVelocity, wantedVelocity, _acceleration * Time.DeltaTime);

        _currentXRotation -= _currentXVelocity * Time.DeltaTime;
        _currentXRotation = ClampVerticalAngle(_currentXRotation);

        return _currentXRotation;
    }

    private float ClampVerticalAngle(float value)
    {
        var maxRadiansAngle = _maxVerticalAngle * Mathf.Deg2Rad;
        return math.clamp(value, -maxRadiansAngle, maxRadiansAngle);
    }
}