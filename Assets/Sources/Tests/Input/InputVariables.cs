using Unity.Entities;

[GenerateAuthoringComponent]
public struct InputVariables : IComponentData
{
    //Cannon
    public float TurnSpeed;
    public float Acceleration;
    public float MaxVerticalAngle;
    public float InputLagPeriod;
    public float ImpulseForce;
}
