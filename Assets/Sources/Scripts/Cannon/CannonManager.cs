using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CannonManager : MonoBehaviour
{
    public static string cannonTagName = "Cannon";

    //private CannonMuzzle _cannonMuzzle;

    //Generar CannonInput si fuera necesario e introducir esta propiedad
    public bool IsFiring => Input.GetButtonDown("Fire1");

    private void Awake()
    {
        //_cannonMuzzle = GetComponent<CannonMuzzle>();

        EnableCannon(true);
        GameManager.CameraController.SetCameraPosition(CameraPosition.Cannon, Vector3.forward);
    }

    private void Update()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        //_cannonMuzzle.IsFireButtonPressed = IsFiring;
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

    public static quaternion GetCannonBarrelRotation()
    {
        return GetBarrelRotation();
    }

    private static quaternion GetBarrelRotation()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var cannonEntity = entityManager.CreateEntityQuery(typeof(CannonTag)).ToEntityArray(Allocator.Temp)[0];

        var barrelEntity = entityManager.GetBuffer<LinkedEntityGroup>(cannonEntity)[2].Value;
        return entityManager.GetComponentData<Rotation>(barrelEntity).Value;
    }
    public static Translation GetCannonBaseTranslation()
    {
        var cannonBaseTranslation = GetCannonBasePosition();
        return new Translation{ Value = new float3(cannonBaseTranslation.x, cannonBaseTranslation.y, cannonBaseTranslation.z)};
    }

    private static float3 GetCannonBasePosition()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var cannonEntity = entityManager.CreateEntityQuery(typeof(CannonTag)).ToEntityArray(Allocator.Temp)[0];

        var baseEntity = entityManager.GetBuffer<LinkedEntityGroup>(cannonEntity)[1].Value;
        return entityManager.GetComponentData<Translation>(baseEntity).Value;
    }

    public static float3 GetCannonBarrelPosition()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var cannonEntity = entityManager.CreateEntityQuery(typeof(CannonTag)).ToEntityArray(Allocator.Temp)[0];

        var barrelEntity = entityManager.GetBuffer<LinkedEntityGroup>(cannonEntity)[2].Value;
        return entityManager.GetComponentData<Translation>(barrelEntity).Value;
    }
}
