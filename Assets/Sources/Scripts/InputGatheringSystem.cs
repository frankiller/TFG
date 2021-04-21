using Unity.Entities;
using UnityEngine;

// in order to circumvent API breakages that do not affect physics, some packages are removed from the project on CI
// any code referencing APIs in com.unity.inputsystem must be guarded behind UNITY_INPUT_SYSTEM_EXISTS
#if UNITY_INPUT_SYSTEM_EXISTS
using UnityEngine.InputSystem;
#endif

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(InitializationSystemGroup))]
class InputGatheringSystem : SystemBase
#if UNITY_INPUT_SYSTEM_EXISTS
    ,
    InputActions.IPlayerActions
#endif
{
    private EntityQuery _playerControllerInputQuery;
    private EntityQuery _playerCannonInputQuery;

#pragma warning disable 649
    private Vector2 _playerMovement;
    private Vector2 _playerLooking;
    private float _playerFiring;
#pragma warning restore 649

#if UNITY_INPUT_SYSTEM_EXISTS
    private InputActions _inputActions;

    protected override void OnStartRunning() => _inputActions.Enable();
    protected override void OnStopRunning() => _inputActions.Disable();

    void InputActions.IPlayerActions.OnMove(InputActions.CallbackContext context) => _playerMovement = context.ReadValue<Vector2>();
    void InputActions.IPlayerActions.OnLook(InputActions.CallbackContext context) => _playerLooking = context.ReadValue<Vector2>();
    void InputActions.IPlayerActions.OnFire(InputActions.CallbackContext context) => _playerFiring = context.ReadValue<float>();
#endif

    protected override void OnCreate()
    {
#if UNITY_INPUT_SYSTEM_EXISTS
        _inputActions = new InputActions();
        _inputActions.Player.SetCallbacks(this);
#endif

        _playerControllerInputQuery = GetEntityQuery(typeof(PlayerControllerInput));
        //_playerCannonInputQuery = GetEntityQuery(typeof(CannonControllerInput));
    }

    protected override void OnUpdate()
    {
        if (_playerControllerInputQuery.CalculateEntityCount() == 0)
        {
            EntityManager.CreateEntity(typeof(PlayerControllerInput));
        }

        _playerControllerInputQuery.SetSingleton(new PlayerControllerInput
        {
            Movement = _playerMovement,
            Looking = _playerLooking
        });

        //if (_playerCannonInputQuery.CalculateEntityCount() == 0)
        //{
        //    EntityManager.CreateEntity(typeof(CannonControllerInput));
        //}

        //_playerCannonInputQuery.SetSingleton(new CannonControllerInput
        //{
        //    Looking = _playerLooking,
        //    Firing = _playerFiring
        //});
    }
}
