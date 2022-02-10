using Unity.Entities;
using UnityEngine.UIElements;

public class UiUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((UiOperationTextData operationTextData, UiSuccessTextData successTextData, UiCrosshairData crosshairData) =>
        {
            if (GetSingleton<SuccessLabelData>().IsVisible)
            {
                operationTextData.Value.style.display = DisplayStyle.None;
                successTextData.Value.style.display = DisplayStyle.Flex;
                crosshairData.Value.style.display = DisplayStyle.None;
            }
            else
            {
                operationTextData.Value.style.display = DisplayStyle.Flex;
                successTextData.Value.style.display = DisplayStyle.None;
                crosshairData.Value.style.display = DisplayStyle.Flex;
            }
        }).WithoutBurst().Run();
    }
}

public class SuccessLabelVisibilitySystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<CannonballHitOnIslandTag>();
    }

    protected override void OnUpdate()
    {
        var successLabelData = GetSingleton<SuccessLabelData>();
        successLabelData.IsVisible = false;
        
        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(GetSingletonEntity<MenuManagerTag>(), successLabelData);
    }
}
