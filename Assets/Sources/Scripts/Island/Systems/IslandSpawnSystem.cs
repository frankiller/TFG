using Unity.Entities;

public class IslandSpawnSystem : SpawnObjectSystemBase<IslandSpawnSettings>
{
    internal override void ConfigureInstance(Entity instance, ref IslandSpawnSettings spawnSettings)
    {
        EntityManager.AddComponentData(instance, new IslandTag());
    }
}