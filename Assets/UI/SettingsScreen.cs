using Unity.Entities;
using UnityEngine.UIElements;

public class SettingsScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SettingsScreen, UxmlTraits> { }

    private DropdownField _gameModeDropdown;
    private DropdownField _gameDifficultyDropdown;
    private Slider _sliderVolume;

    public SettingsScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        _gameModeDropdown = this.Q<DropdownField>("dropdown-game-mode");
        _gameDifficultyDropdown = this.Q<DropdownField>("dropdown-game-difficulty");

        this.Q("button-back")?.RegisterCallback<ClickEvent>(e => SetGameVariables());

        _sliderVolume = this.Q<Slider>("slider-volume");
        _sliderVolume?.RegisterValueChangedCallback(e => SetVolume());

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void SetGameVariables()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var gameManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameManagerTag>());

        if (gameManagerEntityQuery.IsEmptyIgnoreFilter) return;

        var gameManagerEntity = gameManagerEntityQuery.GetSingletonEntity();
        entityManager.AddComponentData(gameManagerEntity, new GameStartData
        {
            Mode = (GameMode) _gameModeDropdown.index,
            Difficulty = (GameDifficulty) _gameDifficultyDropdown.index
        });

        entityManager.SetComponentData(gameManagerEntity, new PlayerSessionScoreData
        {
            Mode = (GameMode) _gameModeDropdown.index,
            Score = 0
        });

    }

    private void SetVolume()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var gameManagerEntityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameManagerTag>());

        if (gameManagerEntityQuery.IsEmptyIgnoreFilter) return;

        entityManager.SetComponentData(gameManagerEntityQuery.GetSingletonEntity(), new VolumeLevelData { Value = _sliderVolume.value });
    }
}
