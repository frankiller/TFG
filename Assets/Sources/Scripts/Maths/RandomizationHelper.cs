using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RandomizationHelper
{
    public static Operation[] GenerateOperationList(Operation correctAnswer, int optionsRange)
    {
        var solutionList = new Operation[optionsRange];
        var correctAnswerIndex = Mathf.FloorToInt(GetFloatInRange(0f, optionsRange));

        for (var i = 0; i < optionsRange; i++)
        {
            if (i == correctAnswerIndex)
            {
                solutionList[i] = correctAnswer;
            }
            else
            {
                var fakeOperation = new Operation();

                RandomizeValue(correctAnswer.Difficulty, out var randomization);

                if (GetBoolean())
                {
                    fakeOperation.Solution = correctAnswer.Solution + randomization;
                }
                else
                {
                    fakeOperation.Solution = correctAnswer.Solution - randomization;
                }

                VerifyFakeSolutionIsUnique(solutionList, fakeOperation);

                solutionList[i] = fakeOperation;
            }
        }

        return solutionList;
    }

    private static void VerifyFakeSolutionIsUnique(Operation[] solutionList, Operation fakeOperation)
    {
        if (!solutionList.Any(solution => solution != null && Math.Abs(solution.Solution - fakeOperation.Solution) < 0.01)) return;

        var randomValue = GetIntegerInRange(1, 10);
        if (GetBoolean())
        {
            fakeOperation.Solution += randomValue;
        }
        else
        {
            fakeOperation.Solution -= randomValue;
        }

        VerifyFakeSolutionIsUnique(solutionList, fakeOperation);
    }

    private static void RandomizeValue(GameDifficulty difficulty, out int randomization)
    {
        var randomizationMultiple = MathsHelper.GetBase10Exp((int)difficulty);

        randomization = difficulty == GameDifficulty.Easy ? GetIntegerInRange(1, 10) : randomizationMultiple;
    }

    public static bool GetBoolean()
    {
        return Mathf.FloorToInt(Random.value * 2) == 0;
    }

    public static float GetFloatInRange(float minValue, float maxValue)
    {
        return Random.Range(minValue, maxValue);
    }

    public static int GetIntegerInRange(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
