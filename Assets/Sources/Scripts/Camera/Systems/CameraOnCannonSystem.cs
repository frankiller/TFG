using Unity.Entities;

public class CameraOnCannonSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endFixedStepSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CannonballHitOnIslandTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var cannonMuzzleEntity = GetSingletonEntity<CannonMuzzleTag>();

        Entities.WithName("CameraOnCannonSystem").ForEach(
            (Entity cameraEntity, ref CameraTargetEntityData cameraTargetEntity) =>
            {
                cameraTargetEntity.Value = cannonMuzzleEntity;
                ecb.SetComponent(cameraEntity, cameraTargetEntity);
            }).Schedule();

        _endFixedStepSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}