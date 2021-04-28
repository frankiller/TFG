using System;
using UnityEngine;

public class CannonMover : MonoBehaviour
{
    [Flags] public enum RotationDirection
    {
        None,
        Horizontal = 1 << 0,
        Vertical = 1 << 1
    }

    [SerializeField] private Transform _cannonBaseTransform;
    [SerializeField] private Vector2 _turnSpeed = new Vector2(1000f, -1000f);
    [SerializeField] private Vector2 _acceleration;
    [SerializeField] private RotationDirection _rotationDirection;
    [SerializeField] private float _maxVerticalAngle;
    [SerializeField] private float _inputLagPeriod;

    private Vector2 _currentRotation;
    private Vector2 _currentVelocity;
    private Vector2 _lastInputEvent;
    private float _inputLagTimer;

    private void OnEnable()
    {
        _currentVelocity = Vector2.zero;
        _inputLagTimer = 0f;
        _lastInputEvent = Vector2.zero;

        Vector3 currentEulerRotation = transform.localEulerAngles;

        if (currentEulerRotation.x >= 180)
        {
            currentEulerRotation.x -= 360;
        }

        currentEulerRotation.x = ClampVerticalAngle(currentEulerRotation.x);

        transform.localEulerAngles = currentEulerRotation;

        _currentRotation = new Vector2(currentEulerRotation.y, currentEulerRotation.x);
    }

    public void Start()
    {
        //if (_cannonBaseTransform != null)
        //{
        //    _cannonBaseTransform.parent = null;
        //}
    }

    public void AimAtMousePosition()
    {
        transform.localEulerAngles = GetRotationToMouse();
    }

    private Vector3 GetRotationToMouse()
    {
        //Debug.DrawRay(transform.position, ray.origin, Color.red);

        return GetRotationToTarget(GetInput());
    }

    private Vector3 GetRotationToTarget(Vector2 target)
    {
        var wantedVelocity = target * _turnSpeed;

        if ((_rotationDirection & RotationDirection.Horizontal) == 0)
        {
            wantedVelocity.x = 0f;
        }

        if ((_rotationDirection & RotationDirection.Vertical) == 0)
        {
            wantedVelocity.y = 0f;
        }

        _currentVelocity = new Vector2(
            Mathf.MoveTowards(_currentVelocity.x, wantedVelocity.x, _acceleration.x * Time.deltaTime),
            Mathf.MoveTowards(_currentVelocity.y, wantedVelocity.y, _acceleration.y * Time.deltaTime));

        _currentRotation += _currentVelocity * Time.deltaTime;
        _currentRotation.y = ClampVerticalAngle(_currentRotation.y);

        return new Vector3(_currentRotation.y, _currentRotation.x, 0f);
    }

    private Vector2 GetInput()
    {
        _inputLagTimer += Time.deltaTime;

        Vector2 input = new Vector2(
            Input.GetAxis("Mouse X"), 
            Input.GetAxis("Mouse Y"));

        if (!Mathf.Approximately(0, input.x) || !Mathf.Approximately(0, input.y) || _inputLagTimer >= _inputLagPeriod)
        {
            _lastInputEvent = input;
            _inputLagTimer = 0f;
        }

        return _lastInputEvent;
    }

    private float ClampVerticalAngle(float angle)
    {
        return Mathf.Clamp(angle, -_maxVerticalAngle, _maxVerticalAngle);
    }
}
