using Unity.Entities;

public class AnswerLabelFadeOutSystem : SystemBase
{
    private Entity _menuManagerEntity;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<GameManagerTag>()
            },
            Any = new[]
            {
                ComponentType.ReadOnly<UpdateUiTag>(),
                ComponentType.ReadOnly<LoadGameTag>()
            }
        }));

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

        _menuManagerEntity = GetSingletonEntity<MenuManagerTag>();
    }

    protected override void OnUpdate()
    {
        var correctAnswerLabelData = GetSingleton<AnswerLabelData>();
        correctAnswerLabelData.Type = AnswerType.None;

        EntityManager.SetComponentData(_menuManagerEntity, correctAnswerLabelData);
    }
}