using Unity.Entities;

public class GameStartSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<LoadMenuTag>();
    }

    protected override void OnUpdate()
    {
        UnloadGameScreen();
        LoadMenuScreen();
    }

    private static void UnloadGameScreen()
    {
        UiHelper.DisableGameUi();
        UiHelper.HideGameScreenCanvasGroup();
        UiHelper.DisablePauseUi();
        UiHelper.DisableFinishedGameUi();
    }

    private static void LoadMenuScreen()
    {
        UiHelper.EnableMenuUi();
    }
}
