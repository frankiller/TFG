using Unity.Entities;

public enum GameMode
{
    SumOrSubstract,
    MultiplyOrDivide,
    Multioperation,
    Algebraic,
    FractionSumOrSubstract,
    FractionMultiplyOrDivide
}

public enum GameDifficulty
{
    Easy,
    Intermediate,
    Hard,
    Test
}

public struct GameStartData : IComponentData
{
    public GameMode Mode;
    public GameDifficulty Difficulty;
}
