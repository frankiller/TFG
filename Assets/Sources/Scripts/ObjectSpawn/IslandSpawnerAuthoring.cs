using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct Ground : IComponentData
{
    public float3 Position;
    public quaternion Orientation;
    public float3 Size;
    public float BevelRadius;
    public float3 Center;
    public float Friction;
    public float Restitution;
}

public class GroundSpawnSettings : SpawnSettings { }

public class IslandSpawnerAuthoring : SpawnObjectAuthoring<GroundSpawnSettings>
{
    public float3 Position = float3.zero;
    public quaternion Orientation = quaternion.identity;
    public float3 Size = float3.zero;
    public float BevelRadius = 0f;
    public float3 Center = float3.zero;
    public float Friction = 0f;
    public float Restitution = 1f;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new Ground
        {
            Position = Position,
            Orientation = Orientation,
            Size = Size,
            BevelRadius = BevelRadius,
            Center = Center,
            Friction = Friction,
            Restitution = Restitution
        });
    }
}

[UpdateBefore(typeof(CannonShootSystem))]
public class IslandSpawnerSystem : SpawnObjectSystemBase<GroundSpawnSettings>
{
    public override void Create(GroundSpawnSettings spawnSettings)
    {
        var entity = GetSingletonEntity<GroundSpawnSettings>();
        var spawnData = GetComponent<Ground>(entity);

        var boxMaterial = new Material
        {
            Friction = spawnData.Friction,
            Restitution = spawnData.Restitution
        };

        var boxCollider = BoxCollider.Create(new BoxGeometry
        {
            Size = spawnData.Size,
            BevelRadius = spawnData.BevelRadius,
            Orientation = spawnData.Orientation,
        }, CollisionFilter.Default, boxMaterial);

        CreatedColliders.Add(boxCollider);

        var createdIsland = CreateStaticBody(boxCollider, spawnData.Position, spawnData.Orientation);
        EntityManager.AddComponentData(createdIsland, new GroundTag());
    }
}
