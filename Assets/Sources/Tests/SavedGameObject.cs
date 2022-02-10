using Unity.Entities;

[GenerateAuthoringComponent]
public class SavedGameObject : IComponentData
{
    public Entity Value;
}
