using Unity.Entities;
using UnityEngine.UIElements;

public class FinishedGameScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FinishedGameScreen, UxmlTraits> { }

    private EntityManager _entityManager;
    private Entity _gameManagerEntity;

    public FinishedGameScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;

        _entityManager = EntityManagerHelper.GetEntityManager();
        _gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();

        RegisterCallbackOnMenuButtons();

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void RegisterCallbackOnMenuButtons()
    {
        this.Q<Button>("button-retry").clicked += RetryGame;
        this.Q<Button>("button-difficulty").clicked += ChooseDifficulty;
        this.Q<Button>("button-exit").clicked += EnableMainMenuScreen;
    }

    private void RetryGame()
    {
        _entityManager.AddComponent<ReloadGameTag>(_gameManagerEntity);

        UiHelper.DisableFinishedGameUi();
        UiHelper.EnableGameUi();
        UiHelper.ShowGameScreenCanvasGroup();
    }

    private static void ChooseDifficulty()
    {
        UiHelper.DisableFinishedGameUi();
        UiHelper.ShowSettingsScreen();
    }

    private void EnableMainMenuScreen()
    {
        _entityManager.AddComponent<ReloadMenuTag>(_gameManagerEntity);

        UiHelper.DisableFinishedGameUi();
        UiHelper.EnableMenuUi();
    }
}
