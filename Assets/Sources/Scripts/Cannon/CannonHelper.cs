using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public static class CannonHelper
{
    public static Entity GetCannonBase(EntityManager entityManager)
    {
        return entityManager.GetBuffer<LinkedEntityGroup>(entityManager.CreateEntityQuery(typeof(CannonTag))
            .GetSingletonEntity())[1].Value;
    }

    public static Entity GetCannonBarrel(EntityManager entityManager)
    {
        return entityManager.GetBuffer<LinkedEntityGroup>(entityManager.CreateEntityQuery(typeof(CannonTag))
            .GetSingletonEntity())[2].Value;
    }

    public static Entity GetCannonMuzzle(EntityManager entityManager)
    {
        return entityManager.GetBuffer<LinkedEntityGroup>(entityManager.CreateEntityQuery(typeof(CannonTag))
            .GetSingletonEntity())[3].Value;
    }

    public static float3 GetCannonBarrelPosition(EntityManager entityManager)
    {
        return entityManager.GetComponentData<Translation>(
            entityManager.GetBuffer<LinkedEntityGroup>(
            entityManager.CreateEntityQuery(typeof(CannonTag)).GetSingletonEntity())[2].Value).Value;
    }

    public static quaternion GetCannonBarrelRotation(EntityManager entityManager)
    {
        return entityManager.GetComponentData<Rotation>(
            entityManager.GetBuffer<LinkedEntityGroup>(
                entityManager.CreateEntityQuery(typeof(CannonTag)).GetSingletonEntity())[2].Value).Value;
    }
}
