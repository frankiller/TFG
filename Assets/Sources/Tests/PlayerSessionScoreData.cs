using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerSessionScoreData : IComponentData
{
    public GameMode Mode;
    public int Score;
}
