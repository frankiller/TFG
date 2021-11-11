using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(CameraPositionSystem))]
[UpdateBefore(typeof(CameraRotationSystem))]
public class CameraTargetDataUpdateSystem : SystemBase
{
    private EntityManager _entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        RequireSingletonForUpdate<CameraTargetEntityData>();
    }

    protected override void OnUpdate()
    {
        var targetEntity = GetSingleton<CameraTargetEntityData>();

        if (!_entityManager.Exists(targetEntity.Value)) return;

        var localToWorld = _entityManager.GetComponentData<LocalToWorld>(targetEntity.Value);
        _entityManager.SetComponentData(GetSingletonEntity<CameraTag>(), new CameraTargetData
        {
            Position = localToWorld.Position,
            Rotation = localToWorld.Rotation,
            Offset = float3.zero
        });
    }
}