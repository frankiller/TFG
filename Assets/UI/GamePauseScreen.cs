using Unity.Entities;
using UnityEngine.UIElements;

public class GamePauseScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<GamePauseScreen, UxmlTraits> {}

    private Slider _sliderVolume;
    private Toggle _toggleVolumeMute;

    public GamePauseScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;

        _sliderVolume = this.Q<Slider>("slider-volume");
        _sliderVolume?.RegisterValueChangedCallback(_ => SetVolume());

        _toggleVolumeMute = this.Q<Toggle>("toggle-mute");
        _toggleVolumeMute.RegisterValueChangedCallback(_ => SetMutedVolume());

        RegisterCallbackOnPauseMenuButtons();

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void SetVolume()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var menuManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MenuManagerTag>());

        if (menuManagerEntityQuery.IsEmpty) return;

        entityManager.SetComponentData(menuManagerEntityQuery.GetSingletonEntity(), new VolumeLevelData { Value = _sliderVolume.value });
    }

    private void SetMutedVolume()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var menuManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MenuManagerTag>());

        if (menuManagerEntityQuery.IsEmpty) return;

        entityManager.SetComponentData(menuManagerEntityQuery.GetSingletonEntity(),
            _toggleVolumeMute.value
                ? new VolumeLevelData {Value = 0.0001f}
                : new VolumeLevelData {Value = _sliderVolume.value});
    }

    private void RegisterCallbackOnPauseMenuButtons()
    {
        this.Q<Button>("button-back-play")?.RegisterCallback<ClickEvent>(_ => EnableGameScreen());
        this.Q<Button>("button-back")?.RegisterCallback<ClickEvent>(_ => EnableMainMenuScreen());
    }

    private static void EnableGameScreen()
    {
        var entityManager = EntityManagerHelper.GetEntityManager();
        var gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();
        
        var chronometerData = entityManager.GetComponentData<ChronometerData>(gameManagerEntity);
        chronometerData.Action = ChronometerAction.Run;
        entityManager.SetComponentData(gameManagerEntity, chronometerData);

        UiHelper.EnableGameUi();
        UiHelper.DisablePauseUi();
    }

    private static void EnableMainMenuScreen()
    {
        var entityManager = EntityManagerHelper.GetEntityManager();
        var gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();
        entityManager.AddComponent<ReloadMenuTag>(gameManagerEntity);
        entityManager.RemoveComponent<InGameTag>(gameManagerEntity);

        UiHelper.DisablePauseUi();
        UiHelper.EnableMenuUi();
    }
}
