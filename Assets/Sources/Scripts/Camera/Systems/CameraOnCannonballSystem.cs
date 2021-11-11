using Unity.Entities;

public class CameraOnCannonballSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endFixedStepSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GetPlayerActionsTag>();
        RequireSingletonForUpdate<CannonballTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var cannonBallEntity = GetSingletonEntity<CannonballTag>();

        Entities.WithName("CameraOnCannonballSystem").ForEach(
            (Entity cameraEntity, ref CameraTargetEntityData cameraTargetEntity) =>
            {
                cameraTargetEntity.Value = cannonBallEntity;
                ecb.SetComponent(cameraEntity, cameraTargetEntity);
            }).Schedule();

        _endFixedStepSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}