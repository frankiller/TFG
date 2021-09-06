using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct InputVariables : IComponentData
{
    //Cannon
    public float2 TurnSpeed;
    public float2 Acceleration;
    public float MaxVerticalAngle;
    public float InputLagPeriod;
    public float ImpulseForce;

    //Trajectory prediction
    public int MaxIterations;
}
