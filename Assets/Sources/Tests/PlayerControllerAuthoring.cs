using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct PlayerControllerComponentData : IComponentData
{
    public float3 Gravity;
    public float MovementSpeed;
    public float MaxMovementSpeed;
    public float RotationSpeed;
    public float MaxSlope; //radians
    public int MaxIterations;
    public float CharacterMass;
    public float SkinWidth;
    public float ContactTolerance;

}

public struct PlayerControllerInput : IComponentData
{
    public float2 Movement;
    public float2 Looking;
}

public class PlayerControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
    }

    private void Start()
    {
    }

    private void Update()
    {

    }
}
