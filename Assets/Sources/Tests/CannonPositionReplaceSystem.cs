using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonPositionReplaceSystem : SystemBase
{
    private EntityManager _entityManager;
    private EntityQuery _cannonEntityQuery;
    private EntityQuery _cannonShootDataQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;

        _cannonEntityQuery = _entityManager.CreateEntityQuery(new ComponentType[] {typeof(CannonTag)});
        _cannonShootDataQuery = GetEntityQuery(new ComponentType [] { typeof(CannonShootData), typeof(IsCorrectTag)});

        RequireForUpdate(_cannonEntityQuery);
    }

    protected override void OnUpdate()
    {
        if (!GameManager.IsFireState() || 
            _cannonEntityQuery.CalculateEntityCount() == 0 || 
            _cannonShootDataQuery.CalculateEntityCount() == 0) return;

        var cannonEntity = _cannonEntityQuery.ToEntityArray(Allocator.Temp)[0];
        using var entities = _cannonShootDataQuery.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            var cannonShootData = EntityManager.GetComponentData<CannonShootData>(entity);

            _entityManager.SetComponentData(cannonEntity, new Translation { Value = cannonShootData.PredictedPosition + new float3(0f, .5f, 0f)});
        }
    }
}
