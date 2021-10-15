using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using World = Unity.Entities.World;

public class CannonSpawnSystem : SpawnObjectSystemBase<CannonSpawnData>
{
    public override void Create(CannonSpawnData spawnData)
    {
        if (!TryGetSingletonEntity<CannonPrefabConversion>(out var cannonPrefabEntity)) { return; }

        var cannonPrefab = GetComponent<CannonPrefabConversion>(cannonPrefabEntity).CannonPrefab;

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var newCannon = entityManager.Instantiate(cannonPrefab);
        entityManager.SetComponentData(newCannon, new Translation { Value = spawnData.Position });
        entityManager.SetComponentData(newCannon, new Rotation { Value = spawnData.Rotation });
        entityManager.AddComponentData(newCannon, new CannonTag());

        var cannonMuzzleEntity = GetBuffer<LinkedEntityGroup>(newCannon)[3].Value;
        entityManager.AddComponentData(cannonMuzzleEntity, new CannonMuzzleTag());

        var attachedObject = new GameObject();
        attachedObject.AddComponent<CannonManager>();

        entityManager.AddComponentObject(newCannon, attachedObject);

        Object.Destroy(attachedObject);
    }
}
