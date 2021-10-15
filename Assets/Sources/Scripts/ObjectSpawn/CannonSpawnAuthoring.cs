using Unity.Entities;
using Unity.Mathematics;

public class CannonSpawnData : SpawnSettings
{
    public float3 Position;
    public quaternion Rotation;
}

public class CannonSpawnAuthoring : SpawnObjectAuthoring<CannonSpawnData>
{
    public float3 Position = float3.zero;
    public quaternion Rotation = quaternion.identity;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new CannonSpawnData
        {
            Position = Position,
            Rotation = Rotation
        });
    }
}
