using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

public interface ISpawnSettings 
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
}

public struct SpawnSettings : IComponentData, ISpawnSettings
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
}

public class SpawnObjectSystem : SpawnObjectSystemBase<SpawnSettings> { }

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
public abstract class SpawnObjectSystemBase<T> : SystemBase where T : struct, IComponentData, ISpawnSettings 
{
    internal virtual void OnBeforeInstantiateObject(ref T spawnSettings) {}

    internal virtual void ConfigureInstance(Entity instance, ref T spawnSettings) {}

    protected override void OnUpdate()
    {
        using var entities = GetEntityQuery(typeof(T)).ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            var spawnSettings = EntityManager.GetComponentData<T>(entity);

            OnBeforeInstantiateObject(ref spawnSettings);

            var instance = EntityManager.Instantiate(spawnSettings.Prefab);
            EntityManager.SetComponentData(instance, new Translation { Value = spawnSettings.Position });
            EntityManager.SetComponentData(instance, new Rotation { Value = spawnSettings.Rotation });
            ConfigureInstance(instance, ref spawnSettings);

            EntityManager.RemoveComponent<T>(entity);
        }
    }
}