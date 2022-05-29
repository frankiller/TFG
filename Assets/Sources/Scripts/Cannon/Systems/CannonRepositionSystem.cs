using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonRepositionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CannonPositionData>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<GameManagerTag>() },
            Any = new []
            {
                ComponentType.ReadOnly<LoadMenuTag>(),
                ComponentType.ReadOnly<LoadGameTag>(),
                ComponentType.ReadOnly<UpdateObjectsPositionTag>()
            }
        }));
    }

    protected override void OnUpdate()
    {
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var cannonEntity = GetSingletonEntity<CannonTag>();
        var cannonBaseEntity = CannonHelper.GetCannonBase(EntityManager);
        var cannonBarrelEntity = CannonHelper.GetCannonBarrel(EntityManager);

        Entities.
            WithName("CannonRepositionSystem").
            WithChangeFilter<CannonPositionData>().
            ForEach((Entity entity, int entityInQueryIndex, in CannonPositionData cannonPosition) =>
        {
            ecb.SetComponent(cannonEntity.Index, cannonEntity, new Translation { Value = cannonPosition.Value + new float3(0f, .5f, 0f) });
            
            ecb.SetComponent(cannonBaseEntity.Index, cannonBaseEntity, new Rotation { Value = quaternion.identity });
            ecb.SetComponent(cannonBarrelEntity.Index, cannonBarrelEntity, new Rotation { Value = quaternion.identity });

            ecb.RemoveComponent<CannonPositionData>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
