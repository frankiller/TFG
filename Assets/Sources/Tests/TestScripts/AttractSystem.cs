using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class AttractSystem : ComponentSystem
{
    private EntityQuery _attractDataQuery;

    protected override void OnCreate()
    {
        _attractDataQuery = GetEntityQuery(ComponentType.ReadOnly<AttractData>());
    }

    protected override unsafe void OnUpdate()
    {
        var attractorEntity = GetSingletonEntity<AttractData>();
        var attractData = GetComponentDataFromEntity<AttractData>(true);

            Entities.ForEach((ref PhysicsVelocity velocity, ref Translation position)
            =>
            {
                float3 diff = attractData[attractorEntity].Center - position.Value;
                float distSqrd = math.lengthsq(diff);
                if (distSqrd < attractData[attractorEntity].MaxDistanceSqrd)
                {
                    // Alter linear velocity
                    velocity.Linear += attractData[attractorEntity].Strength * (diff / math.sqrt(distSqrd));
                }
            });
    }
};