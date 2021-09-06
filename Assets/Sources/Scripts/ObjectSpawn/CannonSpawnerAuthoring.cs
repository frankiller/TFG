using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct CannonTag : IComponentData { }

public class CannonSpawnSettings : SpawnSettings
{
    public GameObject CannonballPrefab;
}

public class CannonSpawnerAuthoring : SpawnObjectAuthoring<CannonSpawnSettings>
{
    public GameObject CannonballPrefab;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new CannonSpawnSettings
        {
            CannonballPrefab = CannonballPrefab
        });
    }
}

public class CannonSpawnerSystem : SpawnObjectSystemBase<CannonSpawnSettings>
{
    public override void Create(CannonSpawnSettings spawnSettings)
    {
        if (!TryGetSingletonEntity<CannonPrefabConversion>(out var cannonPrefabEntity)) { return; }

        var cannonPrefab = GetComponent<CannonPrefabConversion>(cannonPrefabEntity).CannonPrefab;

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var newCannon = entityManager.Instantiate(cannonPrefab);
        entityManager.SetComponentData(newCannon, new Translation { Value = new float3(0f, 1.4f, 0f) });
        entityManager.SetComponentData(newCannon, new Rotation { Value = quaternion.identity});
        entityManager.AddComponentData(newCannon, new CannonTag());

        var attachedObject = new GameObject();
        attachedObject.AddComponent<CannonManager>();
        //attachedObject.AddComponent<CannonMuzzle>().CannonballPrefab = spawnSettings.CannonballPrefab;

        entityManager.AddComponentObject(newCannon, attachedObject);

        Object.Destroy(attachedObject);

        //_entityManager.GetComponentData<>()
    }
}
