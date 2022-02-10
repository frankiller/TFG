using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AttractData : IComponentData
{
    public float3 Center;
    public float MaxDistanceSqrd;
    public float Strength;
}

public class AttractComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float maxDistance = 3;
    public float strength = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var attractData = new AttractData
        {
            Center = transform.position,
            MaxDistanceSqrd = maxDistance * maxDistance,
            Strength = strength
        };

        dstManager.AddComponentData(entity, attractData);
    }
}