using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnSettingsPrefabHolder : IComponentData
{
    public Entity spawnSettingsEntity;
}