using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CameraRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.
            WithName("CameraRotationSystem").
            WithAll<CameraTag>().
            ForEach((ref Rotation rotation, in Translation translation, in CameraTargetData cameraTargetData) =>
            {
                float3 displacement = cameraTargetData.Position - translation.Value;
                var lookRotation = quaternion.LookRotationSafe(cameraTargetData.Position, cameraTargetData.Rotation.value.y);

                //rotation.Value = math.slerp(rotation.Value, cameraTargetData.Rotation, deltaTime);
                rotation.Value = cameraTargetData.Rotation;
            }).Run();
    }
}