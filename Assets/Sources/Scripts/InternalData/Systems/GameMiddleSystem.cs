using Unity.Entities;

[UpdateAfter(typeof(AnswerLabelFadeOutSystem))]
public class GameMiddleSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<LoadGameTag>();
    }

    protected override void OnUpdate()
    {
        SetComponentsOnGameManagerEntity();
        LoadGameScreen();
        UnloadMenuScreen();
    }

    private void SetComponentsOnGameManagerEntity()
    {
        EntityManager.SetComponentData(GetSingletonEntity<GameManagerTag>(), new PlayerGameplayData
        {
            RemainingOperations = GetSingleton<MaxAllowedScoreData>().Value,
            TimeExpended = 0f
        });
    }

    private static void LoadGameScreen()
    {
        UiHelper.EnableGameUi();
        UiHelper.ShowGameScreenCanvasGroup();
    }

    private static void UnloadMenuScreen()
    {
        UiHelper.DisableMenuUi();
    }
}
