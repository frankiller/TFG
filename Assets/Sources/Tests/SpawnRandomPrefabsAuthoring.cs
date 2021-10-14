using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

class SpawnRandomPrefabsAuthoring : SpawnRandomPrefabsAuthoringBase<PrefabSpawnSettings>
{
}

abstract class SpawnRandomPrefabsAuthoringBase<T> : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    where T : struct, IComponentData, IPrefabSpawnSettings
{
    #pragma warning disable 649
    public GameObject prefab;
    public float3 range = new float3(10f);
    public int count;
    #pragma warning restore 649

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnSettings = new T
        {
            Prefab = conversionSystem.GetPrimaryEntity(prefab),
            Position = transform.position,
            Rotation = transform.rotation,
            Range = range,
            Count = count
        };
        Configure(ref spawnSettings);
        dstManager.AddComponentData(entity, spawnSettings);
    }

    internal virtual void Configure(ref T spawnSettings) {}

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.Add(prefab);
}

interface IPrefabSpawnSettings
{
    Entity Prefab { get; set; }
    float3 Position { get; set; }
    quaternion Rotation { get; set; }
    float3 Range { get; set; }
    int Count { get; set; }
}

struct PrefabSpawnSettings : IComponentData, IPrefabSpawnSettings
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
    public float3 Range { get; set; }
    public int Count { get; set; }
}

class SpawnRandomPrefabsSystem : SpawnRandomPrefabsSystemBase<PrefabSpawnSettings>
{
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
abstract class SpawnRandomPrefabsSystemBase<T> : SystemBase where T : struct, IComponentData, IPrefabSpawnSettings
{
    internal virtual int GetRandomSeed(T spawnSettings)
    {
        var seed = 0;
        seed = (seed * 397) ^ spawnSettings.Count;
        seed = (seed * 397) ^ (int)math.csum(spawnSettings.Position);
        seed = (seed * 397) ^ (int)math.csum(spawnSettings.Range);
        return seed;
    }

    internal virtual void OnBeforeInstantiatePrefab(ref T spawnSettings) {}

    internal virtual void ConfigureInstance(Entity instance, ref T spawnSettings) {}

    protected override void OnUpdate()
    {
        // Entities.ForEach in generic system types are not supported
        using var entities = GetEntityQuery(typeof(T)).ToEntityArray(Allocator.TempJob);
        for (int j = 0; j < entities.Length; j++)
        {
            var entity = entities[j];
            var spawnSettings = EntityManager.GetComponentData<T>(entity);

            var count = spawnSettings.Count;

            OnBeforeInstantiatePrefab(ref spawnSettings);

            var instances = new NativeArray<Entity>(count, Allocator.Temp);
            EntityManager.Instantiate(spawnSettings.Prefab, instances);

            var positions = new NativeArray<float3>(count, Allocator.Temp);
            var rotations = new NativeArray<quaternion>(count, Allocator.Temp);
            RandomPointsInRange(spawnSettings.Position, spawnSettings.Rotation, spawnSettings.Range, ref positions, ref rotations, GetRandomSeed(spawnSettings));

            for (int i = 0; i < count; i++)
            {
                var instance = instances[i];
                EntityManager.SetComponentData(instance, new Translation { Value = positions[i] });
                EntityManager.SetComponentData(instance, new Rotation { Value = rotations[i] });
                ConfigureInstance(instance, ref spawnSettings);
            }

            EntityManager.RemoveComponent<T>(entity);
        }
    }

    protected static void RandomPointsInRange(
        float3 center, quaternion orientation, float3 range,
        ref NativeArray<float3> positions, ref NativeArray<quaternion> rotations, int seed = 0)
    {
        var count = positions.Length;
        // initialize the seed of the random number generator
        var random = new Unity.Mathematics.Random((uint)seed);
        for (int i = 0; i < count; i++)
        {
            positions[i] = center + math.mul(orientation, random.NextFloat3(-range, range));
            rotations[i] = math.mul(random.NextQuaternionRotation(), orientation);
        }
    }
}
