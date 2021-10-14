using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum CameraPositionNoEcs
{
    Cannon,
    Cannonball
}

public class CameraController : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _cameraQuery;
    private EntityQuery _cannonMuzzleEntityQuery;
    private EntityQuery _cannonballEntityQuery;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _cameraQuery = _entityManager.CreateEntityQuery(typeof(CameraTag));
        _cannonMuzzleEntityQuery = _entityManager.CreateEntityQuery(typeof(CannonMuzzleTag));
        _cannonballEntityQuery = _entityManager.CreateEntityQuery(typeof(CannonballTag));
    }

    public void SetCameraPosition(CameraPositionNoEcs positionNoEcs, Vector3 cameraDirection = default)
    {
        switch (positionNoEcs)
        {
            case CameraPositionNoEcs.Cannon:
                SetCameraOnCannon();
                break;
            
            case CameraPositionNoEcs.Cannonball:
                SetCameraOnCannonball(cameraDirection);
                break;
        }
    }

    public void ResetCameraPosition()
    {
        SetCameraOnCannon();
    }

    private void SetCameraOnCannon()
    {
        var cameraEntity = _cameraQuery.GetSingletonEntity();
        var cannonMuzzleEntity = _cannonMuzzleEntityQuery.GetSingletonEntity();

        _entityManager.SetComponentData(cameraEntity, new CameraTargetEntityData { Value = cannonMuzzleEntity });
    }

    private void SetCameraOnCannonball(Vector3 cameraDirection)
    {
        var cameraEntity = _cameraQuery.GetSingletonEntity();
        var cannonballEntity = _cannonballEntityQuery.GetSingletonEntity();

        _entityManager.SetComponentData(cameraEntity, new CameraTargetEntityData {Value = cannonballEntity });
    }
}
