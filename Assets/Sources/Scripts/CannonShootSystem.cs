using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Conversion;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Material = Unity.Physics.Material;

[Serializable]
public struct CannonShootData : IComponentData
{
    public float3 Direction;
    public float Force;
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonShootSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;
    private IslandSpawnerAuthoring _islandSpawnerAuthoring;
    private EntityQuery _cannonShootDataQuery;
    private EntityQuery _islandSpawnOptionsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _islandSpawnerAuthoring = new IslandSpawnerAuthoring();
        _cannonShootDataQuery = GetEntityQuery(new ComponentType [] { typeof(CannonShootData) });
        _islandSpawnOptionsQuery = GetEntityQuery(new ComponentType[] {typeof(GroundSpawnSettings)});
    }

    protected override void OnUpdate()
    {
        if (_cannonShootDataQuery.CalculateEntityCount() == 0) {return;} // || _islandSpawnOptionsQuery.CalculateEntityCount() == 0) { return; }

        var fixedDeltaTime = Time.fixedDeltaTime;
        //var groundSpawnSettingsEntity = GetSingletonEntity<GroundSpawnSettings>();
        //var groundSpawnSettings =  _islandSpawnOptionsQuery.GetSingletonEntity();

        //using var entities = _cannonShootDataQuery.ToEntityArray(Allocator.TempJob);
        //foreach (var entity in entities)
        //{
        //    var physicsMass = EntityManager.GetComponentData<PhysicsMass>(entity);
        //    var translation = EntityManager.GetComponentData<Translation>(entity);
        //    var rotation = EntityManager.GetComponentData<Rotation>(entity);
        //    var cannonShootData = EntityManager.GetComponentData<CannonShootData>(entity);

        //    var velocity = cannonShootData.Force / physicsMass.GetEffectiveMass(translation, rotation, cannonShootData.Force, float3.zero) * fixedDeltaTime;
        //    var endPosition = translation.Value + cannonShootData.Direction * velocity * 5f;

        //    EntityManager.AddComponentData(groundSpawnSettings, new Ground
        //    {
        //        Position = endPosition,
        //        Orientation = quaternion.identity,
        //        Size = new float3(20f, 1f, 20f),
        //        BevelRadius = 0f,
        //        Center = float3.zero,
        //        Friction = 0f,
        //        Restitution = 1f
        //    });

        //    _islandSpawnerAuthoring.Convert(groundSpawnSettings, EntityManager, World.GetExistingSystem<GameObjectConversionSystem>());

        //    EntityManager.SetComponentData(groundSpawnSettings, new GroundSpawnSettings { Material = });


        //}

        var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("CannonShootSystem").
            ForEach(
            (Entity entity, int entityInQueryIndex, ref PhysicsVelocity physicsVelocity, 
                ref PhysicsMass physicsMass, ref Translation position, 
                ref Rotation rotation, in CannonShootData cannonShootData) =>
            {
                var velocity = cannonShootData.Force / physicsMass.GetEffectiveMass(position, rotation, cannonShootData.Force, float3.zero) * fixedDeltaTime;
                var endPosition = position.Value + cannonShootData.Direction * velocity * 10f;

                Debug.Log($"X: {endPosition.x} Y: {endPosition.y} Z: {endPosition.z}");

                //var groundSpawnSettingsEntity = GetSingletonEntity<GroundSpawnSettings>();

                physicsVelocity.Linear = math.normalize(cannonShootData.Direction) * cannonShootData.Force;

                ecb.RemoveComponent<CannonShootData>(entityInQueryIndex, entity);
            }).ScheduleParallel();

        _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}
