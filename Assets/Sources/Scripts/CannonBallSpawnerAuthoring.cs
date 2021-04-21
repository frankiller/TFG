using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Collider = Unity.Physics.Collider;
using SphereCollider = Unity.Physics.SphereCollider;

public struct Cannonball : IComponentData
{
    public float Radius;
    public float3 Position;
    public quaternion Rotation;
    public float3 LinearVelocity;
    public float3 AngularVelocity;
    public float Mass;
    public float Friction;
    public float Restitution;
}

public class CannonballSpawnSettings : SpawnSettings { }

public class CannonBallSpawnerAuthoring : SpawnObjectAuthoring<CannonballSpawnSettings>
{
    public float Radius = 1f;
    public float3 Position = float3.zero;
    public quaternion Rotation = quaternion.identity;
    public float3 LinearVelocity = float3.zero;
    public float3 AngularVelocity = float3.zero;
    public float Mass = 1f;

    public float Friction = 0f;
    public float Restitution = 1f;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new Cannonball
        {
            Position = Position,
            Rotation = Rotation,
            Radius = Radius,
            Mass = Mass,
            LinearVelocity = LinearVelocity,
            AngularVelocity = AngularVelocity,
            Friction = Friction,
            Restitution = Restitution
        });
    }
}

public class CannonBallSpawnerSystem : SpawnObjectSystemBase<CannonballSpawnSettings>
{
    public override void Create(CannonballSpawnSettings spawnSettings)
    {
        var entity = GetSingletonEntity<CannonballSpawnSettings>();
        var spawnData = GetComponent<Cannonball>(entity);

        var spMaterial = new Material
        {
            Friction = spawnData.Friction,
            Restitution = spawnData.Restitution
        };

        BlobAssetReference<Collider> spCollider = SphereCollider.Create(
            new SphereGeometry
            {
                Center = float3.zero,
                Radius = spawnData.Radius
            }, CollisionFilter.Default, spMaterial);
         
        CreatedColliders.Add(spCollider);

        var cannonBallEntity = CreateDynamicBody(spCollider, spawnData.Position, spawnData.Rotation, spawnData.LinearVelocity,
            spawnData.AngularVelocity, spawnData.Mass);

        EntityManager.AddComponentData(cannonBallEntity, new CannonShootData
        {
            Direction = new float3(0f, 1f, 1f),
            Force = 10f
        });
    }
}
