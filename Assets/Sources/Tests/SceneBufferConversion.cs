using Unity.Entities;

[GenerateAuthoringComponent]
public struct SceneBufferConversion : IBufferElementData
{
    public GameScenes Scene;
}
