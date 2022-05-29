using System;
using Unity.Entities;

[Serializable]
public struct PlayerTimeBuffer : IBufferElementData
{
    public GameMode Mode;
    public GameDifficulty Difficulty;
    public float MinimumTimeExpended;

    public static implicit operator GameMode(PlayerTimeBuffer e)
    {
        return e.Mode;
    }

    public static implicit operator GameDifficulty(PlayerTimeBuffer e)
    {
        return e.Difficulty;
    }

    public static implicit operator float(PlayerTimeBuffer e)
    {
        return e.MinimumTimeExpended;
    }

    public static implicit operator PlayerTimeBuffer(GameMode e)
    {
        return new PlayerTimeBuffer { Mode = e };
    }

    public static implicit operator PlayerTimeBuffer(GameDifficulty e)
    {
        return new PlayerTimeBuffer { Difficulty = e };
    }

    public static implicit operator PlayerTimeBuffer(float e)
    {
        return new PlayerTimeBuffer { MinimumTimeExpended = e };
    }
}

[Serializable]
public struct PlayerTimeBufferWrapper
{
    public PlayerTimeBuffer[] Buffer;
}
