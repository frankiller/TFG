using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(CameraPositionSystem))]
public class CameraTargetDataUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<CameraTargetEntityData>();
    }

    protected override void OnUpdate()
    {
        var targetEntity = GetSingleton<CameraTargetEntityData>();

        if (!EntityManager.Exists(targetEntity.Value)) return;

        var localToWorld = EntityManager.GetComponentData<LocalToWorld>(targetEntity.Value);
        EntityManager.SetComponentData(GetSingletonEntity<CameraTag>(), new CameraTargetData
        {
            Position = localToWorld.Position,
            Rotation = localToWorld.Rotation,
            Offset = float3.zero
        });
    }
}