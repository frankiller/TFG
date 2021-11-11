using Unity.Entities;

[GenerateAuthoringComponent]
public struct LeftMouseButtonData : IComponentData
{
    public bool IsClicked;
}