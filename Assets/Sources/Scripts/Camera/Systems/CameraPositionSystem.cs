using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CameraPositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.
            WithName("CameraPositionSystem").
            WithAll<CameraTag>().
            ForEach((ref Translation translation, in CameraTargetData cameraTargetData) =>
            {
                translation.Value = cameraTargetData.Position - cameraTargetData.Offset;
            }).Run();
    }
}

