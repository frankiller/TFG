using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class InputSystemConversor : MonoBehaviour
{
    private InputAction _playerPointAction;

    private void Awake()
    {
        _playerPointAction = GetComponent<PlayerInput>().actions["Point"];
    }

    public void ReadMouseMovementInput()
    {
        var entityManager = EntityManagerHelper.GetEntityManager();
        var gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();
        
        var inputData = _playerPointAction.ReadValue<Vector2>();
        entityManager.AddComponentData(gameManagerEntity, new RawInputData
        {
            XInput = inputData.x,
            YInput = inputData.y
        });
    }

    public void ReadTouchscreenTapInput()
    {
        EntityManagerHelper.GetEntityManager()
            .AddComponent<FireActionTag>(EntityManagerHelper.GetGameManagerEntity());
    }

    public void ShowPauseMenu()
    {
        var entityManager = EntityManagerHelper.GetEntityManager();
        var gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();
        
        var chronometerData = entityManager.GetComponentData<ChronometerData>(gameManagerEntity);
        chronometerData.Action = ChronometerAction.Pause;
        entityManager.SetComponentData(gameManagerEntity, chronometerData);
        
        UiHelper.DisableGameUi();
        UiHelper.EnablePauseUi();
    }
}
