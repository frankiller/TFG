using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public struct ShootData : IComponentData
{
    public float3 InitialPosition;
    public float3 DesiredPosition;
    public float Speed;
    public float3 Range;
}

public class ShootAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float3 Range;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ShootData
        {
            InitialPosition = transform.position,
            DesiredPosition = transform.position,
            Speed = math.length(Range) * 0.001f,
            Range = Range
        });
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
public class ShootSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorld;

    protected override void OnCreate()
    {
        _buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(ShootData)
            }
        }));
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("ShootData")
            .WithBurst()
            .ForEach((ref ShootData shoot, ref PhysicsVelocity velocity) =>
            {
                velocity.Linear = math.lerp(shoot.InitialPosition, shoot.Range, shoot.Speed);
            }).Schedule();

        _buildPhysicsWorld.AddInputDependency(Dependency);
    }
}
