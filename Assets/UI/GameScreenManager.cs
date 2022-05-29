using Unity.Entities;
using UnityEngine.UIElements;

public class GameScreenManager : VisualElement
{
    public new class UxmlFactory : UxmlFactory<GameScreenManager, UxmlTraits> { }

    public GameScreenManager()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;

        AddComponentsToMenuManagerEntity();

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void AddComponentsToMenuManagerEntity()
    {
        var entityManager = EntityManagerHelper.GetEntityManager();
        var menuManagerEntity = EntityManagerHelper.GetMenuManagerEntity();
        var gameScreen = this.Q("GameScreen");

        var chronometerLabel = gameScreen.Q<Label>("label-chrono");
        if (entityManager.HasComponent<UiChronometerTextData>(menuManagerEntity))
        {
            chronometerLabel.text = entityManager.GetComponentObject<UiChronometerTextData>(menuManagerEntity).Value.text;
        }

        //Comprobar si en vez de un new ...Data es mejor hacer un GetComponentObject y actualizar su valor
        entityManager.AddComponentObject(menuManagerEntity, new UiChronometerTextData { Value = chronometerLabel });

        var operationLabel = gameScreen.Q<Label>("label-operation");
        if (entityManager.HasComponent<UiOperationTextData>(menuManagerEntity))
        {
            operationLabel.text = entityManager.GetComponentObject<UiOperationTextData>(menuManagerEntity).Value.text;
        }

        entityManager.AddComponentObject(menuManagerEntity, new UiOperationTextData { Value = operationLabel });

        var scoreLabel = gameScreen.Q<Label>("label-score");
        if (entityManager.HasComponent<UiScoreTextData>(menuManagerEntity))
        {
            scoreLabel.text = entityManager.GetComponentObject<UiScoreTextData>(menuManagerEntity).Value.text;
        }
        else
        {
            scoreLabel.text += entityManager.GetComponentData<MaxAllowedScoreData>(
                EntityManagerHelper.GetGameManagerEntity()).Value.ToString();
        }

        entityManager.AddComponentObject(menuManagerEntity, new UiScoreTextData { Value = scoreLabel });

        var succeedLabel = gameScreen.Q<Label>("label-succeed");
        entityManager.AddComponentObject(menuManagerEntity, new UiSuccessTextData { Value = succeedLabel });

        var succeedNextIslandLabel = gameScreen.Q<Label>("label-next-island");
        entityManager.AddComponentObject(menuManagerEntity, new UiSuccessNextIslandTextData { Value = succeedNextIslandLabel });

        var failureLabel = gameScreen.Q<Label>("label-failure");
        entityManager.AddComponentObject(menuManagerEntity, new UiFailureTextData { Value = failureLabel });

        var crosshair = gameScreen.Q("crosshair");
        entityManager.AddComponentObject(menuManagerEntity, new UiCrosshairData { Value = crosshair });
    }
}
