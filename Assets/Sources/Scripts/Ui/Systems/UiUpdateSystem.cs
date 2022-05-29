using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class UiUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<UpdateLabelTag>();
    }

    protected override void OnUpdate()
    {
        var isCannonFired = EntityManager.HasComponent<CannonFiredTag>(GetSingletonEntity<GameManagerTag>());

        Entities.ForEach((UiOperationTextData operationTextData, UiSuccessTextData successTextData,
            UiSuccessNextIslandTextData successNextIslandTextData, UiFailureTextData failureTextData,
            UiCrosshairData crosshairData, AnswerLabelData answerLabelData) =>
        {
            if (isCannonFired)
            {
                operationTextData.Value.style.display = DisplayStyle.None;
                successTextData.Value.style.display = DisplayStyle.None;
                successNextIslandTextData.Value.style.display = DisplayStyle.None;
                failureTextData.Value.style.display = DisplayStyle.None;
                crosshairData.Value.style.display = DisplayStyle.None;

                UiHelper.HideGameScreenCanvasGroup();
            }
            else
            {
                switch (answerLabelData.Type)
                {
                    case AnswerType.Correct:
                        operationTextData.Value.style.display = DisplayStyle.None;

                        ShowLabel(successTextData);
                        ShowLabel(successNextIslandTextData);

                        failureTextData.Value.style.display = DisplayStyle.None;
                        crosshairData.Value.style.display = DisplayStyle.None;

                        UiHelper.ShowGameScreenCanvasGroup();
                        break;
                    case AnswerType.Incorrect:
                        operationTextData.Value.style.display = DisplayStyle.None;
                        successTextData.Value.style.display = DisplayStyle.None;

                        ShowLabel(failureTextData);

                        crosshairData.Value.style.display = DisplayStyle.None;

                        UiHelper.ShowGameScreenCanvasGroup();
                        break;
                    default:
                        operationTextData.Value.style.display = DisplayStyle.Flex;
                        successTextData.Value.style.display = DisplayStyle.None;
                        failureTextData.Value.style.display = DisplayStyle.None;
                        crosshairData.Value.style.display = DisplayStyle.Flex;
                        break;
                }
            }
        }).WithoutBurst().Run();

        EntityManager.RemoveComponent<UpdateLabelTag>(GetSingletonEntity<GameManagerTag>());
    }

    private static void ShowLabel(UiLabel labelData)
    {
        labelData.Value.style.display = DisplayStyle.Flex;

        DoAnimation(labelData);
    }

    private static void DoAnimation(UiLabel label)
    {
        var labelPosition = label.Value.style.top.value.value;
        label.Value.experimental.animation
            .Start(
                new StyleValues { top = labelPosition - 100, opacity = 0 },
                new StyleValues { top = labelPosition, opacity = 1 }, 2000).Ease(Easing.InQuad)
            .OnCompleted(() => label.Value.experimental.animation
                .Start(
                    new StyleValues { opacity = 1 },
                    new StyleValues { opacity = 1 }, 1000)
                .OnCompleted(() => label.Value.experimental.animation
                    .Start(
                        new StyleValues { top = labelPosition, opacity = 1 },
                        new StyleValues { top = labelPosition + 100, opacity = 0 }, 2000).Ease(Easing.OutQuad)))
            ;
    }
}
