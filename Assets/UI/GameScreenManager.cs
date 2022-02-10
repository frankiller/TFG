using Unity.Entities;
using UnityEngine.UIElements;

public class GameScreenManager : VisualElement
{
    public new class UxmlFactory : UxmlFactory<GameScreenManager, UxmlTraits> { }

    private EntityManager _entityManager;
    private EntityQuery _menuManagerEntityQuery;

    private VisualElement _gameScreen;
    private VisualElement _pauseScreen;

    public GameScreenManager()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _menuManagerEntityQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<MenuManagerTag>());

        if (_menuManagerEntityQuery.IsEmptyIgnoreFilter) return;

        _gameScreen = this.Q("GameScreen");
        _pauseScreen = this.Q("GamePauseScreen");

        AddComponentsToMenuManagerEntity();

        RegisterCallbackOnMenuButton();
        RegisterCallbackOnPauseMenuButtons();

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void AddComponentsToMenuManagerEntity()
    {
        var operationLabel = _gameScreen?.Q<Label>("label-operation");
        var succeedLabel = _gameScreen?.Q<Label>("label-succeed");
        var crosshair = _gameScreen?.Q("crosshair");

        var menuManagerEntity = _menuManagerEntityQuery.GetSingletonEntity();

        _entityManager.AddComponentObject(menuManagerEntity, new UiOperationTextData {Value = operationLabel, Radius = 15});
        _entityManager.AddComponentObject(menuManagerEntity, new UiSuccessTextData {Value = succeedLabel});
        _entityManager.AddComponentObject(menuManagerEntity, new UiCrosshairData {Value = crosshair});
    }

    private void RegisterCallbackOnMenuButton()
    {
        _gameScreen?.Q<Button>("button-menu")?.RegisterCallback<ClickEvent>(e => EnableGamePauseScreen());
    }

    private void RegisterCallbackOnPauseMenuButtons()
    {
        _pauseScreen?.Q<Button>("button-back-play")?.RegisterCallback<ClickEvent>(e => EnableGameScreen());
        _pauseScreen?.Q<Button>("button-back")?.RegisterCallback<ClickEvent>(e => EnableMainMenuScreen());
    }

    private void EnableMainMenuScreen()
    {
        _entityManager.AddComponentData(_entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameManagerTag>()).GetSingletonEntity(), new ReloadMenuSceneTag());
    }

    private void EnableGamePauseScreen()
    {
        _gameScreen.style.display = DisplayStyle.None;
        _pauseScreen.style.display = DisplayStyle.Flex;
    }

    private void EnableGameScreen()
    {
        _gameScreen.style.display = DisplayStyle.Flex;
        _pauseScreen.style.display = DisplayStyle.None;
    }
}
