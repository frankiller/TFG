using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CannonMuzzleSystem))]
[UpdateAfter(typeof(GameEndSystem))]
public class ScoreLabelUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<UiScoreTextData>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<GameManagerTag>() },
            Any = new[]
            {
                ComponentType.ReadOnly<CannonballHitOnIslandTag>(),
                ComponentType.ReadOnly<LoadGameTag>()
            }
        }));
    }

    protected override void OnUpdate()
    {
        var scoreLabelData = EntityManager.GetComponentObject<UiScoreTextData>(GetSingletonEntity<MenuManagerTag>());
        scoreLabelData.Value.text = "Operaciones restantes: " + GetSingleton<PlayerGameplayData>().RemainingOperations;
    }
}
