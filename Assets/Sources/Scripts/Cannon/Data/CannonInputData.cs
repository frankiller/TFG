using Unity.Entities;

[GenerateAuthoringComponent]
public struct CannonInputData : IComponentData
{
    public float TurnSpeed;
    public float TurnAcceleration;
    public float TurnMaxVerticalAngle;
    public float TurnInputLagPeriod;
    public float ShootImpulseForce;
    public float TargetDetectionDistance;
}
