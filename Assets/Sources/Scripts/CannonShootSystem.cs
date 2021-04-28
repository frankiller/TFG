using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[Serializable]
public struct CannonShootData : IComponentData
{
    public float3 Direction;
    public float Force;
    public float3 PredictedPosition;
}

public struct CannonballTag : IComponentData { }

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonShootSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;
    private IslandSpawnerAuthoring _islandSpawnerAuthoring;
    private EntityQuery _cannonShootDataQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _cannonShootDataQuery = GetEntityQuery(new ComponentType [] { typeof(CannonShootData) });
    }

    protected override void OnUpdate()
    {
        if (_cannonShootDataQuery.CalculateEntityCount() == 0) {return;}

        var groundSpawnSettingsEntity = GetSingletonEntity<GroundSpawnSettings>();
        var spawnData = GetComponent<Ground>(groundSpawnSettingsEntity);

        using var entities = _cannonShootDataQuery.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            var cannonShootData = EntityManager.GetComponentData<CannonShootData>(entity);

            EntityManager.AddComponentData(groundSpawnSettingsEntity, new Ground
            {
                Position = cannonShootData.PredictedPosition,
                Orientation = spawnData.Orientation,
                Size = spawnData.Size,
                BevelRadius = spawnData.BevelRadius,
                Center = spawnData.Center,
                Friction = spawnData.Friction,
                Restitution = spawnData.Restitution
            });

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<IslandSpawnerSystem>().Create(null);
        }

        var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("CannonShootSystem").
            ForEach(
            (Entity entity, int entityInQueryIndex, ref PhysicsVelocity physicsVelocity, in CannonShootData cannonShootData) =>
            {
                physicsVelocity.Linear = math.normalize(cannonShootData.Direction) * cannonShootData.Force;

                ecb.RemoveComponent<CannonShootData>(entityInQueryIndex, entity);
                
            }).ScheduleParallel();

        _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}
