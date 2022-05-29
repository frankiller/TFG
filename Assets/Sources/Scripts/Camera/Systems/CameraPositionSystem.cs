using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CameraPositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var isCannonball = EntityManager.HasComponent<CannonballTag>(GetSingleton<CameraTargetEntityData>().Value);

        Entities.
            WithName("CameraPositionSystem").
            WithAll<CameraTag>().
            ForEach((ref Translation translation, ref Rotation rotation, in CameraTargetData cameraTargetData) =>
            {
                if (isCannonball)
                {
                    translation.Value = math.lerp(translation.Value, cameraTargetData.Position, 0.10f);

                    var relativePos = cameraTargetData.Position - translation.Value;
                    rotation.Value = quaternion.LookRotationSafe(relativePos, math.up());
                }
                else
                {
                    translation.Value = cameraTargetData.Position;
                    rotation.Value = cameraTargetData.Rotation;
                }
            }).ScheduleParallel();
    }
}

