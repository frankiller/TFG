using Unity.Mathematics;
using UnityEngine;

public static class TrajectoryPrediction
{
    public static float3 Predict(float3 currentPosition, Vector3 direction)
    {
        var gravity = -Physics.gravity.y;
        var velocity = direction.y;

        var time = (velocity + Mathf.Sqrt(Mathf.Pow(velocity, 2) + 2 * gravity * currentPosition.y)) / gravity;

        var x = direction.x * time;
        var z = direction.z * time;
        var y = (direction.y * time) - (gravity * Mathf.Pow(time, 2) / 2);

        return new float3(x + currentPosition.x, y + currentPosition.y, z + currentPosition.z);
    }
}
