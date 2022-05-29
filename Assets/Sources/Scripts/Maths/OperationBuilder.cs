public static class OperationBuilder
{
    public static Operation SumOrMinus()
    {
        var operation = new Operation();
        var operand1 = operation.FirstOperand;
        var operand2 = operation.SecondOperand;

        if (RandomizationHelper.GetBoolean())
        {
            operation.Solution = operand1 + operand2;
            operation.Definition = $"{operand1} + {operand2}";
            return operation;
        }

        operation.Solution = operand1 - operand2;
        operation.Definition = $"{operand1} - {operand2}";
        return operation;
    }

    public static Operation MultiplyOrDivide()
    {
        var operation = new Operation();
        var operand1 = operation.FirstOperand;
        var multiplicand = operation.Multiplicand;

        if (RandomizationHelper.GetBoolean())
        {
            operation.Solution = operand1 * multiplicand;
            operation.Definition = $"{operand1} × {multiplicand}";
            return operation;
        }

        operation.Solution = multiplicand;
        operation.Definition = $"{operand1 * multiplicand} ÷ {operand1}";
        return operation;
    }

    public static Operation MultiOperand()
    {
        var operation = new Operation();
        var operand1 = operation.FirstOperand;
        var operand2 = operation.SecondOperand;
        var operand3 = operation.Multiplicand;

        if (RandomizationHelper.GetBoolean())
        {
            operation.Solution = (operand1 + operand2) * operand3;
            operation.Definition = $"({operand1} + {operand2}) × {operand3}";
            return operation;
        }

        operation.Solution = (operand1 - operand2) * operand3;
        operation.Definition = $"({operand1} - {operand2}) × {operand3}";
        return operation;
    }

    public static Operation AlgebraicOperation()
    {
        var operation = new Operation();
        var operand1 = operation.Multiplicand;
        var operand2 = operation.FirstOperand;
        var answer = operation.SecondOperand;

        operation.Solution = answer;
        operation.Definition = RandomizationHelper.GetBoolean()
            ? $"{operand1}x + {operand2} = {operand1 * answer + operand2}"
            : $"{operand1}x - {operand2} = {operand1 * answer - operand2}";
        return operation;
    }

    public static Operation FractionSumOrMinus()
    {
        var operation = new Operation();
        var difficulty = MathsHelper.GetBase10Exp((int)operation.Difficulty);
        var operand1 = new Fraction(MathsHelper.GetFloatRounded(RandomizationHelper.GetFloatInRange(0.1f, 10f) * difficulty) / difficulty);
        var operand2 = new Fraction(MathsHelper.GetFloatRounded(RandomizationHelper.GetFloatInRange(0.1f, 10f) * difficulty) / difficulty);

        if (RandomizationHelper.GetBoolean())
        {
            operation.Solution = (operand1 + operand2).ToFloat();
            operation.Definition = $"{operand1.Definition} + {operand2.Definition}";
            return operation;
        }

        operation.Solution = (operand1 - operand2).ToFloat();
        operation.Definition = $"{operand1.Definition} - {operand2.Definition}";
        return operation;
    }

    public static Operation FractionMultiplyOrDivide()
    {
        var operation = new Operation();
        var difficulty = MathsHelper.GetBase10Exp((int)operation.Difficulty);
        var operand1 = new Fraction(MathsHelper.GetFloatRounded(RandomizationHelper.GetFloatInRange(0.1f, 10f) * difficulty) / difficulty);
        var operand2 = new Fraction(MathsHelper.GetFloatRounded(RandomizationHelper.GetFloatInRange(0.1f, 10f) * difficulty) / difficulty);

        if (RandomizationHelper.GetBoolean())
        {
            operation.Solution = (operand1 * operand2).ToFloat();
            operation.Definition = $"{operand1} × {operand2}";
            return operation;
        }

        operation.Solution = (operand1 / operand2).ToFloat();
        operation.Definition = $"{operand1} ÷ {operand2}";
        return operation;
    }
}