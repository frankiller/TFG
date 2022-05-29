using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnObjectAuthoring : SpawnObjectAuthoringBase<SpawnSettings> {}

public abstract class SpawnObjectAuthoringBase<T> : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs 
    where T : struct, IComponentData, ISpawnSettings
{
    public GameObject Prefab;
    public float3 Position;
    public quaternion Rotation;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //dstManager.SetName(entity, $"{name}");

        var spawnSettings = new T
        {
           Prefab = conversionSystem.GetPrimaryEntity(Prefab),
           Position = Position,
           Rotation = Rotation
        };

        Configure(ref spawnSettings);

        dstManager.AddComponentData(entity, spawnSettings);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.Add(Prefab);

    internal virtual void Configure(ref T spawnSettings) { }
}
