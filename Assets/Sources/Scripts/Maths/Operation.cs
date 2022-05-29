public class Operation
{
    public int FirstOperand => GetRandomOperator;
    public int SecondOperand => GetRandomOperator;
    public int Multiplicand => GetRandomMultiplicand;

    public virtual float Solution { get; set; }
    public virtual string Definition { get; set; }

    public GameDifficulty Difficulty { get; }

    public Operation()
    {
        Difficulty = EntityManagerHelper.GetEntityManager()
            .GetComponentData<GameStartData>(EntityManagerHelper
                .GetGameManagerEntity()).Difficulty;
    }

    private int GetRandomOperator => RandomizationHelper.GetIntegerInRange(1, MathsHelper.GetBase10Exp((int)Difficulty + 2));

    private int GetRandomMultiplicand => RandomizationHelper.GetIntegerInRange(2, MathsHelper.GetBase10Exp((int)Difficulty + 1));
}
