using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class GameEndSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<GameManagerTag>() },
            Any = new[]
            {
                ComponentType.ReadOnly<ReloadMenuTag>(),
                ComponentType.ReadOnly<ReloadGameTag>()
            }
        }));
    }

    protected override void OnUpdate()
    {
        var gameManagerEntity = GetSingletonEntity<GameManagerTag>();

        var playerSessionScore = GetSingleton<PlayerGameplayData>();
        playerSessionScore.RemainingOperations = GetSingleton<MaxAllowedScoreData>().Value;
        playerSessionScore.TimeExpended = 0;
        EntityManager.SetComponentData(gameManagerEntity, playerSessionScore);

        var chronometerData = GetSingleton<ChronometerData>();
        chronometerData.Action = ChronometerAction.Stop;
        EntityManager.SetComponentData(gameManagerEntity, chronometerData);

        EntityManager.RemoveComponent<CannonFiredTag>(gameManagerEntity);
        EntityManager.RemoveComponent<UpdateUiTag>(gameManagerEntity);
        EntityManager.RemoveComponent<Timer>(GetSingletonEntity<MenuManagerTag>());
    }
}
