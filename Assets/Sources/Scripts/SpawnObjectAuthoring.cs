using Unity.Entities;
using UnityEngine;

public interface ISpawnSettingsEntity
{
    public Entity spawnSettingsEntity { get; set; }
}

public interface ISpawnSettings 
{
    public float Friction { get; set; }
    public float Restitution { get; set; }
}

public abstract class SpawnSettings : IComponentData
{
    public Material Material;
}

public struct ObjectSpawnedTag : IComponentData { }

public struct ToSpawn : IComponentData { }

public struct GroungTag : IComponentData { }

public abstract class SpawnObjectAuthoring<T> : MonoBehaviour, IConvertGameObjectToEntity where T : SpawnSettings, new()
{
    public Material Material;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.SetName(entity, $"{name}");

        T spawnSettings = new T
        {
            Material = Material
        };

        Configure(ref spawnSettings);

        dstManager.AddComponentData(entity, spawnSettings);
    }

    public virtual void Configure(ref T spawnSettings) { }
}
