using Unity.Entities;

public class CannonSpawnSystem : SpawnObjectSystemBase<CannonSpawnSettings>
{
    internal override void ConfigureInstance(Entity instance, ref CannonSpawnSettings spawnSettings)
    {
        EntityManager.AddComponentData(instance, new CannonTag());

        var cannonMuzzleEntity = CannonHelper.GetCannonMuzzle(EntityManager);
        EntityManager.AddComponentData(cannonMuzzleEntity, new CannonMuzzleTag());
        EntityManager.SetComponentData(GetSingletonEntity<CameraTag>(), new CameraTargetEntityData { Value = cannonMuzzleEntity });
    }
}
