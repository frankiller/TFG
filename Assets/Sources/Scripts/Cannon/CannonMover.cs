using System;
using UnityEngine;

public class CannonMover : ScriptableObject
{
    [Flags] public enum RotationDirection
    {
        None,
        Horizontal = 1 << 0,
        Vertical = 1 << 1
    }

    public Vector2 _turnSpeed = new Vector2(100f, -100f);
    public Vector2 _acceleration = new Vector2(100f, 100f);
    public RotationDirection RotationDir;
    public float _maxVerticalAngle;
    public float _inputLagPeriod;

    private float _currentZEulerRotation;
    private float _currentYRotation;
    private float _currentZRotation;

    private float _currentYVelocity;
    private float _currentZVelocity;

    private float _lastXInputEvent;
    private float _lastYInputEvent;

    private float _inputLagTimer;

    private float GetXInput
    {
        get
        {
            _inputLagTimer += Time.deltaTime;

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
            _inputLagTimer += Time.deltaTime;

            var input = Input.GetAxis("Mouse Y");

            if (Mathf.Approximately(0, input) && !(_inputLagTimer >= _inputLagPeriod)) return _lastYInputEvent;
            _lastYInputEvent = input;
            _inputLagTimer = 0f;

            return _lastYInputEvent;
        }
    }

    private void OnEnable()
    {
        _currentYVelocity = 0f;
        _currentZVelocity = 0f;
        
        _lastXInputEvent = 0f;
        _lastYInputEvent = 0f;

        _inputLagTimer = 0f;

        _currentZEulerRotation = CannonManager.GetCannonBarrelRotation().value.x;

        if (_currentZEulerRotation >= 180)
        {
            _currentZEulerRotation -= 360;
        }

        _currentZEulerRotation = ClampVerticalAngle(_currentZEulerRotation);

        _currentYRotation = CannonManager.GetCannonBarrelRotation().value.y;
        _currentZRotation = _currentZEulerRotation;
    }

    public void Awake()
    {
        //CannonManager.GetCannonBarrelRotation().localRotation = Quaternion.Euler(0f, 0f, ClampVerticalAngle(_currentZEulerRotation));
        //CannonManager.GetCannonBaseTranslation().localRotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    public void AimAtMousePosition()
    {
        //CannonManager.GetCannonBaseTranslation().localRotation = GetYRotationToMouse() * Quaternion.Euler(-90f, 0f, 0f);
        
        //CannonManager.GetCannonBarrelRotation().localRotation = GetYRotationToMouse() * Quaternion.Euler(0f, 0f, 90f) * GetZRotationToMouse();
    }

    private Quaternion GetZRotationToMouse()
    {
        //if ((RotationDir & RotationDirection.Vertical) == 0)
        //{
        //    return Quaternion.Euler(0f, 0f, -_currentZRotation);
        //}

        var wantedVelocity = GetYInput * _turnSpeed.y;
        _currentZVelocity = Mathf.MoveTowards(_currentZVelocity, wantedVelocity, _acceleration.y * Time.deltaTime);

        _currentZRotation += _currentZVelocity * Time.deltaTime;
        _currentZRotation = ClampVerticalAngle(_currentZRotation);

        return Quaternion.Euler(0f, 0f, -_currentZRotation);
    }

    private Quaternion GetYRotationToMouse()
    {
        //if ((RotationDir & RotationDirection.Horizontal) == 0)
        //{
        //    return Quaternion.Euler(0f, _currentYRotation, 0f);
        //}

        var wantedVelocity = GetXInput * _turnSpeed.x;
        _currentYVelocity = Mathf.MoveTowards(_currentYVelocity, wantedVelocity, _acceleration.x * Time.deltaTime);

        _currentYRotation += _currentYVelocity * Time.deltaTime;

        return Quaternion.Euler(0f, _currentYRotation, 0f);
    }

    private float ClampVerticalAngle(float angle)
    {
        return Mathf.Clamp(angle, -_maxVerticalAngle, _maxVerticalAngle);
    }
}
