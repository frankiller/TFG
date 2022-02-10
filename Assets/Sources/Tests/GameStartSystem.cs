using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum GameMode
{
    SumSubstract,
    MultiplyDivide,
    Multioperation,
    Algebraic
}

public enum GameDifficulty
{
    Test,
    Easy,
    Intermediate,
    Hard
}

public struct GameStartData : IComponentData
{
    public GameMode Mode;
    public GameDifficulty Difficulty;
}

public enum GameScenes
{
    Menu,
    FirstIsland
}

public class GameStartSystem : SystemBase
{
    //private SceneSystem _sceneSystem;
    //private EntityQuery _subScenesEntityQuery;
    protected override void OnCreate()
    {
        base.OnCreate();

        //_sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        //_subScenesEntityQuery = GetEntityQuery(ComponentType.ReadOnly<SceneReference>());

        RequireSingletonForUpdate<LoadMenuSceneTag>();
    }

    protected override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name != "Menu") return;

        LoadMenuScreen();
    }

    private void LoadMenuScreen()
    {
        var menuUi = GameManager.Instance.MenuUI.rootVisualElement;
        menuUi.Q("MainMenuScreen").style.display = DisplayStyle.Flex;
    }
}

public class GameMiddleSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        
        RequireSingletonForUpdate<LoadGameSceneTag>();
    }

    protected override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name != "Menu") return;

        LoadGameScreen();
        UnloadMenuScreen();
    }

    private void LoadGameScreen()
    {
        var gameUi = GameManager.Instance.GameUI.rootVisualElement;
        gameUi.Q("GameScreen").style.display = DisplayStyle.Flex;
    }

    private void UnloadMenuScreen()
    {
        var menuUi = GameManager.Instance.MenuUI.rootVisualElement;
        menuUi.Q("MainMenuScreen").style.display = DisplayStyle.None;
    }
}
