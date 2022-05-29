using Unity.Entities;
using UnityEngine.UIElements;

public class SettingsScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SettingsScreen, UxmlTraits> { }

    private EntityManager _entityManager;
    private Entity _gameManagerEntity;

    private DropdownField _gameModeDropdown;
    private DropdownField _gameDifficultyDropdown;
    private Label _maxScoreLabel;
    private Slider _sliderVolume;

    public SettingsScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;

        _entityManager = EntityManagerHelper.GetEntityManager();
        _gameManagerEntity = EntityManagerHelper.GetGameManagerEntity();

        RegisterCallbacks();

        UpdateMaxScoreLabel();

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void RegisterCallbacks()
    {
        _gameModeDropdown = this.Q<DropdownField>("dropdown-game-mode");
        _gameModeDropdown.RegisterValueChangedCallback(_ => UpdateMaxScoreLabel());

        _gameDifficultyDropdown = this.Q<DropdownField>("dropdown-game-difficulty");
        _gameDifficultyDropdown.RegisterValueChangedCallback(_ => UpdateMaxScoreLabel());
        _maxScoreLabel = this.Q<Label>("label-max-score");

        this.Q<Button>("button-back").clicked += SetGameVariables;
        this.Q<Button>("button-play").clicked += PlayWithGameVariables;

        _sliderVolume = this.Q<Slider>("slider-volume");
        _sliderVolume?.RegisterValueChangedCallback(_ => SetVolume());
    }

    private void UpdateMaxScoreLabel()
    {
        UiHelper.UpdateSettingsScreenUi(_maxScoreLabel, (GameMode)_gameModeDropdown.index, (GameDifficulty)_gameDifficultyDropdown.index);
    }

    private void SetGameVariables()
    {
        _entityManager.SetComponentData(_gameManagerEntity, new GameStartData
        {
            Mode = (GameMode)_gameModeDropdown.index,
            Difficulty = (GameDifficulty)_gameDifficultyDropdown.index
        });

        _entityManager.SetComponentData(_gameManagerEntity, new PlayerGameplayData
        {
            RemainingOperations = _entityManager.GetComponentData<MaxAllowedScoreData>(_gameManagerEntity).Value
        });
    }

    private void PlayWithGameVariables()
    {
        SetGameVariables();

        _entityManager.AddComponent<ReloadGameTag>(_gameManagerEntity);

        UiHelper.HideSettingsScreen();
        UiHelper.EnableGameUi();
        UiHelper.ShowGameScreenCanvasGroup();
    }

    private void SetVolume()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var menuManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MenuManagerTag>());

        if (menuManagerEntityQuery.IsEmpty) return;

        entityManager.SetComponentData(menuManagerEntityQuery.GetSingletonEntity(), new VolumeLevelData { Value = _sliderVolume.value });
    }
}
