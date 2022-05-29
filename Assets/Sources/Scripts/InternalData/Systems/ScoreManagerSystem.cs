using Unity.Entities;

public class ScoreManagerSystem : SystemBase
{
    private Entity _gameManagerEntity;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<UpdateObjectsPositionTag>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<MenuManagerTag>()
            },
            None = new[] { ComponentType.ReadOnly<Timer>() }
        }));
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        _gameManagerEntity = GetSingletonEntity<GameManagerTag>();
    }

    protected override void OnUpdate()
    {
        var playerSessionScore = GetSingleton<PlayerGameplayData>();

        if (playerSessionScore.RemainingOperations > 0) return;

        StopChronometer();

        UpdatePlayerTime(playerSessionScore.TimeExpended);

        if (GetSingleton<AnswerLabelData>().Type == AnswerType.None)
        {
            SetUiChanges();
        }
    }

    private void StopChronometer()
    {
        var chronometerData = GetSingleton<ChronometerData>();
        chronometerData.Action = ChronometerAction.Pause;
        EntityManager.SetComponentData(_gameManagerEntity, chronometerData);
    }

    private void UpdatePlayerTime(float timeExpended)
    {
        var playerTimeBuffer = GetBuffer<PlayerTimeBuffer>(_gameManagerEntity).Reinterpret<PlayerTimeBuffer>();
        var gameStartData = GetSingleton<GameStartData>();
        var modeExists = false;

        if (playerTimeBuffer.IsEmpty)
        {
            playerTimeBuffer.Add(new PlayerTimeBuffer
            {
                Mode = gameStartData.Mode,
                Difficulty = gameStartData.Difficulty,
                MinimumTimeExpended = timeExpended
            });
        }
        else
        {
            for (var i = 0; i < playerTimeBuffer.Length; i++)
            {
                if (playerTimeBuffer[i].Mode != gameStartData.Mode || playerTimeBuffer[i].Difficulty != gameStartData.Difficulty) continue;
                modeExists = true;

                if (playerTimeBuffer[i].MinimumTimeExpended < timeExpended) continue;

                var score = playerTimeBuffer[i];
                score.MinimumTimeExpended = timeExpended;
                playerTimeBuffer[i] = score;
            }

            if (!modeExists)
            {
                playerTimeBuffer.Add(new PlayerTimeBuffer
                {
                    Mode = gameStartData.Mode,
                    Difficulty = gameStartData.Difficulty,
                    MinimumTimeExpended = timeExpended
                });
            }
        }
    }

    private void SetUiChanges()
    {
        EntityManager.RemoveComponent<InGameTag>(_gameManagerEntity);

        UiHelper.DisableGameUi();
        UiHelper.EnableFinishedGameUi();
        UiHelper.HideGameScreenCanvasGroup();
    }
}
