using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetPrefabConversion : IComponentData
{
    public Entity TargetPrefab;
}
