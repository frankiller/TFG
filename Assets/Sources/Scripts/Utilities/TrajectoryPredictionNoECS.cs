using Unity.Mathematics;
using UnityEngine;

public class TrajectoryPredictionNoECS : ScriptableObject
{
    public float3 Predict(float t, float3 currentPosition, Vector3 force)
    {
        float x = force.x * t;
        float z = force.z * t;
        float y = (force.y * t) - (-Physics.gravity.y * Mathf.Pow(t, 2) / 2);

        return new float3(x + currentPosition.x, y + currentPosition.y, z + currentPosition.z);
    }
}
