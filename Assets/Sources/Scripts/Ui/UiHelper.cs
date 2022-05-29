using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiHelper
{
    public static void EnableMenuUi()
    {
        GameManager.Instance.MenuUi.enabled = true;
        GameManager.Instance.MenuUi.rootVisualElement.Q("MainMenuScreen").style.display = DisplayStyle.Flex;

        //UnityEngine.Debug.Log($"EnableMenuUi {GameManager.Instance.MenuUi.rootVisualElement.Q("MainMenuScreen").style.display.value}");
    }

    public static void DisableMenuUi()
    {
        if (!GameManager.Instance.MenuUi.isActiveAndEnabled) return;

        GameManager.Instance.MenuUi.rootVisualElement.Q("MainMenuScreen").style.display = DisplayStyle.None;

        //UnityEngine.Debug.Log($"DisableMenuUi {GameManager.Instance.MenuUi.rootVisualElement.Q("MainMenuScreen").style.display.value}");

        GameManager.Instance.MenuUi.enabled = false;

    }

    public static void EnableGameUi()
    {
        GameManager.Instance.GameUi.enabled = true;
        GameManager.Instance.GameUi.rootVisualElement.Q("GameScreen").style.display = DisplayStyle.Flex;
    }

    public static void DisableGameUi()
    {
        if (!GameManager.Instance.GameUi.isActiveAndEnabled) return;

        GameManager.Instance.GameUi.rootVisualElement.Q("GameScreen").style.display = DisplayStyle.None;
        GameManager.Instance.GameUi.enabled = false;
    }

    public static void EnablePauseUi()
    {
        GameManager.Instance.PauseGameUi.enabled = true;
        GameManager.Instance.PauseGameUi.rootVisualElement.Q("GamePauseScreen").style.display = DisplayStyle.Flex;
    }

    public static void DisablePauseUi()
    {
        GameManager.Instance.PauseGameUi.rootVisualElement.Q("GamePauseScreen").style.display = DisplayStyle.None;
        GameManager.Instance.PauseGameUi.enabled = false;
    }

    public static void EnableFinishedGameUi()
    {
        var finishedGameUi = GameManager.Instance.FinishedGameUi;
        finishedGameUi.enabled = true;
        finishedGameUi.rootVisualElement.Q("FinishedGameScreen").style.display = DisplayStyle.Flex;
        finishedGameUi.rootVisualElement.Q("FinishedGameScreen").Q<Button>("button-retry").style.display = DisplayStyle.Flex;
    }

    public static void UpdateSettingsScreenUi(Label labelElement, GameMode gameMode, GameDifficulty gameDifficulty)
    {
        var playerTimeBuffer = EntityManagerHelper.GetEntityManager()
            .GetBuffer<PlayerTimeBuffer>(EntityManagerHelper.GetGameManagerEntity()).AsNativeArray().ToArray();

        var elapsedTime = playerTimeBuffer
            .FirstOrDefault(playerTime => playerTime.Mode == gameMode && playerTime.Difficulty == gameDifficulty)
            .MinimumTimeExpended;
        
        if (elapsedTime > 0)
        {
            var secondsRemainder = (int)(Mathf.Floor(elapsedTime % 60 * 100) / 100.0f);
            var minutes = (int)(elapsedTime / 60) % 60;

            labelElement.text = $"Tiempo mínimo conseguido: {minutes}m {secondsRemainder}s";
        }
        else
        {
            labelElement.text = "Tiempo mínimo aún no registrado";
        }
    }

    public static void DisableFinishedGameUi()
    {
        var finishedGameUi = GameManager.Instance.FinishedGameUi;
        finishedGameUi.rootVisualElement.Q("FinishedGameScreen").Q<Button>("button-retry").style.display = DisplayStyle.None;
        finishedGameUi.rootVisualElement.Q("FinishedGameScreen").style.display = DisplayStyle.None;

        finishedGameUi.enabled = false;
    }

    public static void ShowSettingsScreen()
    {
        var menuUi = GameManager.Instance.MenuUi;
        menuUi.enabled = true;
        menuUi.rootVisualElement.Q("SettingsScreen").style.display = DisplayStyle.Flex;
        menuUi.rootVisualElement.Q("SettingsScreen").Q("optional-button-container").style.display = DisplayStyle.None;
        menuUi.rootVisualElement.Q("SettingsScreen").Q("button-play").style.display = DisplayStyle.Flex;
    }

    public static void HideSettingsScreen()
    {
        var menuUi = GameManager.Instance.MenuUi;
        menuUi.rootVisualElement.Q("SettingsScreen").style.display = DisplayStyle.None;
        menuUi.rootVisualElement.Q("SettingsScreen").Q("optional-button-container").style.display = DisplayStyle.Flex;
        menuUi.rootVisualElement.Q("SettingsScreen").Q("button-play").style.display = DisplayStyle.None;
        menuUi.enabled = false;
    }

    public static void HideGameScreenCanvasGroup()
    {
        GameManager.Instance.ScreenControlsCanvasGroup.alpha = 0f;
        GameManager.Instance.ScreenControlsCanvasGroup.interactable = false;
    }

    public static void ShowGameScreenCanvasGroup()
    {
        GameManager.Instance.ScreenControlsCanvasGroup.alpha = 1f;
        GameManager.Instance.ScreenControlsCanvasGroup.interactable = true;
    }
}
