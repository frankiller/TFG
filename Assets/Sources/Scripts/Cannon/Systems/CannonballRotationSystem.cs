using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonballRotationSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<CannonballTag>();
    }

    protected override void OnUpdate()
    {
        var cameraEntity = GetSingletonEntity<CameraTag>();
        var cameraPosition = EntityManager.GetComponentData<LocalToWorld>(cameraEntity);
        var deltaTime = Time.DeltaTime;

        Entities.
            WithName("CameraPositionSystem").
            WithAll<CannonballTag>().
            ForEach((ref Rotation rotation) =>
            {
                rotation.Value = math.slerp(
                    rotation.Value,
                    math.mul(cameraPosition.Rotation, quaternion.Euler(cameraPosition.Rotation.value.x + 45, 0, 0)),
                    deltaTime);
            }).ScheduleParallel();
    }
}
