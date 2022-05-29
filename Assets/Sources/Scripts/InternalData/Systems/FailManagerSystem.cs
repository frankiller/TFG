using Unity.Entities;

public class FailManagerSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<CannonballMisshitTag>();
    }

    protected override void OnUpdate()
    {
        var chronometerData = GetSingleton<ChronometerData>();
        chronometerData.Action = ChronometerAction.IncreaseTime;

        SetComponent(GetSingletonEntity<GameManagerTag>(), chronometerData);
    }
}
