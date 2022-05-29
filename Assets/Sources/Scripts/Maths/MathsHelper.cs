using Unity.Mathematics;

public static class MathsHelper
{
    public static int GetBase10Exp(float value) => (int)math.exp10(value);

    public static float GetFloatRounded(float value) => math.round(value);
}
