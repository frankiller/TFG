using UnityEngine;
using Random = UnityEngine.Random;

public class Questions
{
    public int OptionsRange = 5;
    public int[] AnswerList;
    public string OperationText;
    
    public Questions()
    {
        AnswerList = new int[OptionsRange];
    }

    public int SumOrMinusOperation()
    {
        var operand1 = Mathf.FloorToInt(Random.value * 100);
        var operand2 = Mathf.FloorToInt(Random.value * 100);

        var isSumOrSubtractionOperation = Mathf.FloorToInt(Random.value * 2);
        if (isSumOrSubtractionOperation == 0)
        {
            OperationText = $"{operand1} + {operand2}";
            return operand1 + operand2;
        }

        OperationText = $"{operand1} - {operand2}";
        return operand1 - operand2;
    }

    public int MultiplyOperation()
    {
        var operand1 = Mathf.FloorToInt(Random.value * 50);
        var operand2 = Random.Range(2, 10);

        OperationText = $"{operand1} * {operand2}";
        return operand1 * operand2;
    }

    public int MultiOperandOperation()
    {
        var operand1 = Mathf.FloorToInt(Random.value * 30);
        var operand2 = Mathf.FloorToInt(Random.value * 20);
        var operand3 = Random.Range(2, 10);

        var isSumOrSubtractionOperation = Mathf.FloorToInt(Random.value * 2);
        if (isSumOrSubtractionOperation == 0)
        {
            OperationText = $"({operand1} + {operand2}) * {operand3}";
            return (operand1 + operand2) * operand3;
        }

        OperationText = $"({operand1} - {operand2}) * {operand3}";
        return (operand1 - operand2) * operand3;
    }

    public int AlgebraicOperation()
    {
        var operand1 = Random.Range(2, 10);
        var operand2 = Mathf.FloorToInt(Random.value * 80);
        var answer = Mathf.FloorToInt(Random.value * 30);

        var isSumOrSubtractionOperation = Mathf.FloorToInt(Random.value * 2);
       
        OperationText = isSumOrSubtractionOperation == 0 
            ? $"{operand1}x + {operand2} = {operand1 * answer + operand2}" 
            : $"{operand1}x - {operand2} = {operand1 * answer - operand2}";

        return answer;
    }

    public int[] RandomAnswerGenerator(int answer)
    {
        ClearAnswerList();

        var correctAnswerIndex = Mathf.FloorToInt(Random.value * OptionsRange);

        for (var i = 0; i < OptionsRange; i++)
        {
            if (i == correctAnswerIndex)
            {
                AnswerList[i] = answer;
            }
            else
            {
                int fakeAnswer;
                var randomization = Mathf.FloorToInt(Random.value * 10);
                if (randomization == 0)
                {
                    randomization++;
                }

                var randomizeOperation = Mathf.FloorToInt(Random.value * 2);
                if (randomizeOperation == 0)
                {
                    fakeAnswer = answer + randomization;
                }
                else
                {
                    fakeAnswer = answer - randomization;
                }

                AnswerList[i] = fakeAnswer;
            }
        }

        return AnswerList;
    }

    private void ClearAnswerList()
    {
        AnswerList = new int[OptionsRange];
    }
}
