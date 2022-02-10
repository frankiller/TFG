using Unity.Entities;
using UnityEngine.UIElements;

public class NewGameScreen : VisualElement
{
    private DropdownField _gameModeDropdown;
    private DropdownField _gameDifficultyDropdown;

    public new class UxmlFactory : UxmlFactory<NewGameScreen, UxmlTraits> { }

    public NewGameScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        _gameModeDropdown = this.Q<DropdownField>("dropdown-game-mode");
        _gameDifficultyDropdown = this.Q<DropdownField>("dropdown-game-difficulty");

        this.Q("button-play")?.RegisterCallback<ClickEvent>(e => EnablePlayGame());

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void EnablePlayGame()
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

        entityManager.AddComponentData(gameManagerEntity, new InitializeSystemsTag());
    }
}
