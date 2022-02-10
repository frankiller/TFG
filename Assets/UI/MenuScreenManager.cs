using Unity.Entities;
using UnityEngine.UIElements;

public class MenuScreenManager : VisualElement
{
    private VisualElement _mainMenuScreen;
    private VisualElement _settingsScreen;
    private VisualElement _creditsScreen;

    public new class UxmlFactory : UxmlFactory<MenuScreenManager, UxmlTraits> { }

    public MenuScreenManager()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        _mainMenuScreen = this.Q("MainMenuScreen");
        _mainMenuScreen?.Q("button-play")?.RegisterCallback<ClickEvent>(e => EnablePlayGame());
        _mainMenuScreen?.Q("button-settings")?.RegisterCallback<ClickEvent>(e => EnableSettingsScreen());
        _mainMenuScreen?.Q("button-credits")?.RegisterCallback<ClickEvent>(e => EnableCreditsScreen());

        _settingsScreen = this.Q("SettingsScreen");
        _settingsScreen?.Q("button-back")?.RegisterCallback<ClickEvent>(e => EnableMainMenuScreen());

        _creditsScreen = this.Q("CreditsScreen");
        _creditsScreen?.Q("button-back")?.RegisterCallback<ClickEvent>(e => EnableMainMenuScreen());

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void EnablePlayGame()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var gameManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameManagerTag>());

        if (gameManagerEntityQuery.IsEmptyIgnoreFilter) return;

        entityManager.AddComponentData(gameManagerEntityQuery.GetSingletonEntity(), new LoadGameSceneTag());
        entityManager.AddComponentData(gameManagerEntityQuery.GetSingletonEntity(), new InitializeSystemsTag());
    }

    private void EnableMainMenuScreen()
    {
        _mainMenuScreen.style.display = DisplayStyle.Flex;
        _settingsScreen.style.display = DisplayStyle.None;
        _creditsScreen.style.display = DisplayStyle.None;
    }

    private void EnableSettingsScreen()
    {
        _mainMenuScreen.style.display = DisplayStyle.None;
        _settingsScreen.style.display = DisplayStyle.Flex;
        _creditsScreen.style.display = DisplayStyle.None;
    }

    private void EnableCreditsScreen()
    {
        _mainMenuScreen.style.display = DisplayStyle.None;
        _settingsScreen.style.display = DisplayStyle.None;
        _creditsScreen.style.display = DisplayStyle.Flex;
    }
}
