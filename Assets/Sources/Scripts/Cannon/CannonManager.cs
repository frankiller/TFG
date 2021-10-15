using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CannonManager : MonoBehaviour
{
    public static string cannonTagName = "Cannon";

    //Generar CannonInput si fuera necesario e introducir esta propiedad
    public bool IsFiring => Input.GetButtonDown("Fire1");

    private void Awake()
    {
        EnableCannon(true);
        GameManager.CameraController.SetCameraPosition(CameraPositionNoEcs.Cannon);
    }

    private void Update()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGameOver())
        {
            EnableCannon(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.EndGame();
        }
    }

    public static void EnableCannon(bool state)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var cannonEntity = entityManager.CreateEntityQuery(typeof(CannonTag)).ToEntityArray(Allocator.Temp)[0];

        if (state)
        {
            entityManager.RemoveComponent<Disabled>(cannonEntity);
        }
        else
        {
            entityManager.AddComponentData(cannonEntity, new Disabled());
        }
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
