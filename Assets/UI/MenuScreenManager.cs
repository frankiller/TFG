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
        _mainMenuScreen?.Q("button-play")?.RegisterCallback<ClickEvent>(_ => EnablePlayGame());
        _mainMenuScreen?.Q("button-settings")?.RegisterCallback<ClickEvent>(_ => EnableSettingsScreen());
        _mainMenuScreen?.Q("button-credits")?.RegisterCallback<ClickEvent>(_ => EnableCreditsScreen());

        _settingsScreen = this.Q("SettingsScreen");
        _settingsScreen?.Q("button-back")?.RegisterCallback<ClickEvent>(_ => EnableMainMenuScreen());

        _creditsScreen = this.Q("CreditsScreen");
        _creditsScreen?.Q("button-back")?.RegisterCallback<ClickEvent>(_ => EnableMainMenuScreen());

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private static void EnablePlayGame()
    {
        var entityManager = EntityManagerHelper.GetEntityManager();
        var gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();
        entityManager.AddComponentData(gameManagerEntity, new LoadGameTag());
        entityManager.AddComponentData(gameManagerEntity, new InitializeGameSystemsTag());
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
