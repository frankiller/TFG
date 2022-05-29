using Unity.Entities;

public class DifficultyUpdaterSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<InitializeGameSystemsTag>();
    }

    protected override void OnUpdate()
    {
        var gameStartData = GetSingleton<GameStartData>();
        var operationRange = gameStartData.Difficulty switch
        {
            GameDifficulty.Easy => 3,
            GameDifficulty.Intermediate => 7,
            GameDifficulty.Hard => 11,
            _ => 1
        };

        EntityManager.AddComponentData(GetSingletonEntity<GameManagerTag>(), new OperationRangeData { Value = operationRange });
    }
}