using UnityEngine.UIElements;

public class GamePauseScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<GamePauseScreen, UxmlTraits> {}

    public GamePauseScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
}
