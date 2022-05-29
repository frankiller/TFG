using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonMoverSystem : SystemBase
{
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

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GetPlayerActionsTag>();
        RequireSingletonForUpdate<InGameTag>();
        RequireSingletonForUpdate<RawInputData>();
    }

    protected override void OnStartRunning()
    {
        var inputVariables = GetComponent<CannonInputData>(GetSingletonEntity<CannonInputData>());
        _turnSpeed = inputVariables.TurnSpeed;
        _acceleration = inputVariables.TurnAcceleration;
        _maxVerticalAngle = inputVariables.TurnMaxVerticalAngle;
        _inputLagPeriod = inputVariables.TurnInputLagPeriod;

        _currentYVelocity = 0f;
        _currentXVelocity = 0f;

        _lastXInputEvent = 0f;
        _lastYInputEvent = 0f;

        _inputLagTimer = 0f;

        var barrelRotation = CannonHelper.GetCannonBarrelRotation(EntityManager).value;
        _currentXEulerRotation = new Quaternion(barrelRotation.x, barrelRotation.y, barrelRotation.z, barrelRotation.w).eulerAngles.x;

        if (_currentXEulerRotation >= 180)
        {
            _currentXEulerRotation -= 360;
        }

        _currentXEulerRotation = ClampVerticalAngle(_currentXEulerRotation);

        _currentYRotation = CannonHelper.GetCannonBarrelRotation(EntityManager).value.y;
        _currentXRotation = _currentXEulerRotation;
    }

    protected override void OnUpdate()
    {
        var rawInputData = GetSingleton<RawInputData>();
        var yRotation = GetYRotationToMouse(GetYInput(rawInputData.XInput));

        var baseEntity = CannonHelper.GetCannonBase(EntityManager);
        var fromRotation = EntityManager.GetComponentData<Rotation>(baseEntity).Value;
        var toRotation = quaternion.Euler(0f, yRotation, 0f);

        EntityManager.SetComponentData(baseEntity, new Rotation { Value = math.nlerp(fromRotation, toRotation, Time.DeltaTime * 5f)});

        var barrelEntity = CannonHelper.GetCannonBarrel(EntityManager);
        fromRotation = EntityManager.GetComponentData<Rotation>(barrelEntity).Value;
        toRotation = quaternion.Euler(GetXRotationToMouse(GetXInput(rawInputData.YInput)), yRotation, 0f);

        EntityManager.SetComponentData(barrelEntity, new Rotation { Value = math.nlerp(fromRotation, toRotation, Time.DeltaTime * 5f)});

        //Esto no tiene sentido. Buscar una forma de coger el input distinta
        rawInputData.XInput = 0f;
        rawInputData.YInput = 0f;
        EntityManager.SetComponentData(GetSingletonEntity<GameManagerTag>(), rawInputData);
    }

    private float GetXInput(float xInput)
    {
        _inputLagTimer += Time.DeltaTime;

        //UnityEngine.Debug.Log($"yInput {xInput}");

        if (Mathf.Approximately(0, xInput) && !(_inputLagTimer >= _inputLagPeriod)) return _lastXInputEvent;
        _lastXInputEvent = xInput;
        _inputLagTimer = 0f;

        //UnityEngine.Debug.Log($"_lastXInputEvent: {_lastXInputEvent}");

        return _lastXInputEvent;
    }

    private float GetYInput(float yInput)
    {
        _inputLagTimer += Time.DeltaTime;

        //UnityEngine.Debug.Log($"yInput {yInput}");

        if (Mathf.Approximately(0, yInput) && !(_inputLagTimer >= _inputLagPeriod)) return _lastYInputEvent;
        _lastYInputEvent = yInput;
        _inputLagTimer = 0f;

        //UnityEngine.Debug.Log($"_lastYInputEvent: {_lastYInputEvent}");

        return _lastYInputEvent;
    }

    private float GetYRotationToMouse(float yInput)
    {
        _currentYVelocity = Mathf.MoveTowards(_currentYVelocity, yInput * _turnSpeed, _acceleration * Time.DeltaTime);

        _currentYRotation += _currentYVelocity * Time.DeltaTime;

        return _currentYRotation;
    }

    private float GetXRotationToMouse(float xInput)
    {
        _currentXVelocity = Mathf.MoveTowards(_currentXVelocity, xInput * _turnSpeed, _acceleration * Time.DeltaTime);

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
