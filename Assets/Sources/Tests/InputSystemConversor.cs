using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class InputSystemConversor : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.UI.Enable();
    }

    public void ReadMouseMovementInput()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var gameManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameManagerTag>());

        if (gameManagerEntityQuery.IsEmptyIgnoreFilter) return;

        var gameManagerEntity = gameManagerEntityQuery.GetSingletonEntity();
        var inputData = _playerInputActions.Player.Look.ReadValue<Vector2>();
        
        entityManager.AddComponentData(gameManagerEntity, new RawInputData
        {
            XInput = inputData.x,
            YInput = inputData.y
        });
    }

    public void ReadMouseClickInput()
    {
        Debug.Log("Click event");
    }
}
