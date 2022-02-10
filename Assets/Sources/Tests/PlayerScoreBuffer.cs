using System;
using Unity.Entities;

[Serializable]
public struct PlayerScoreBuffer : IBufferElementData
{
    public GameMode Mode;
    public int MaxScore;
}
