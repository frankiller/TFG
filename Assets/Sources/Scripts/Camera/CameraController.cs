using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum CameraPosition
{
    Cannon,
    Cannonball
}

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    private EntityManager _entityManager;
    private EntityQuery _cannonEntityQuery;
    private EntityQuery _cannonballEntityQuery;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _cannonEntityQuery = _entityManager.CreateEntityQuery(typeof(CannonTag));
        _cannonballEntityQuery = _entityManager.CreateEntityQuery(typeof(CannonballTag));
    }

    public void SetCameraPosition(CameraPosition position, Vector3 cameraDirection = default, Vector3 cameraRotation = default)
    {
        var camera = _cameraTransform.gameObject;
        var followEntityScript = camera.GetComponent<FollowEntity>();

        if (followEntityScript == null)
        {
            followEntityScript = camera.AddComponent<FollowEntity>();
        }

        switch (position)
        {
            case CameraPosition.Cannon:
                followEntityScript.entityToFollow = _entityManager.GetBuffer<LinkedEntityGroup>(_cannonEntityQuery.GetSingletonEntity())[3].Value;
                break;
            
            case CameraPosition.Cannonball:
                followEntityScript.entityToFollow = _cannonballEntityQuery.GetSingletonEntity();
                followEntityScript.offset = cameraDirection * 4;
                break;
            
            default:
                _cameraTransform.position = Vector3.zero;
                break;
        }
    }

    public void ResetCameraPosition()
    {
        _cameraTransform.parent = null;
        _cameraTransform.position = Vector3.zero;
    }
}
