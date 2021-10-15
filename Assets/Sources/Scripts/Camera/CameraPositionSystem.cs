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
        Entities.
            WithName("CameraPositionSystem").
            WithAll<CameraTag>().
            ForEach((ref Translation translation, in CameraTargetData cameraTargetData) =>
            {
                translation.Value = cameraTargetData.Position - cameraTargetData.Offset;
            }).Run();
    }
}

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

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(CameraPositionSystem))]
[UpdateBefore(typeof(CameraRotationSystem))]
public class ProcessCameraTargetData : SystemBase
{
    private EntityManager _entityManager;
    private EntityQuery _cameraQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _cameraQuery = _entityManager.CreateEntityQuery(typeof(CameraTag));

        RequireForUpdate(_cameraQuery);
    }

    protected override void OnUpdate()
    {
        var cameraEntity = _cameraQuery.GetSingletonEntity();
        var targetEntity = GetComponent<CameraTargetEntityData>(cameraEntity);

        if (!_entityManager.Exists(targetEntity.Value)) return;

        var localToWorld = _entityManager.GetComponentData<LocalToWorld>(targetEntity.Value);
        _entityManager.SetComponentData(cameraEntity, new CameraTargetData
        {
            Position = localToWorld.Position,
            Rotation = localToWorld.Rotation,
            Offset = float3.zero
        });
    }
}

public class SetCameraTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
}
