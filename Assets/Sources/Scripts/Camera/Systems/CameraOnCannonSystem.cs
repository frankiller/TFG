using Unity.Entities;

public class CameraOnCannonSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endFixedStepSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endFixedStepSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<GameManagerTag>() },
            Any = new[]
            {
                ComponentType.ReadOnly<CannonballHitOnIslandTag>(),
                ComponentType.ReadOnly<CannonballMisshitTag>()
            }
        }));
    }

    protected override void OnUpdate()
    {
        var ecb = _endFixedStepSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var cannonMuzzleEntity = GetSingletonEntity<CannonMuzzleTag>();

        Entities.
            WithName("CameraOnCannonSystem").
            ForEach((Entity cameraEntity, int entityInQueryIndex, ref CameraTargetEntityData cameraTargetEntity) =>
            {
                cameraTargetEntity.Value = cannonMuzzleEntity;
                ecb.SetComponent(entityInQueryIndex, cameraEntity, cameraTargetEntity);
            }).ScheduleParallel();

        _endFixedStepSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}