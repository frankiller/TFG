using Unity.Entities;
using Unity.Mathematics;
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
    private EntityQuery _cannonballEntityQuery;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _cannonballEntityQuery = _entityManager.CreateEntityQuery(new ComponentType[] {typeof(CannonballTag)});

        SetCameraPosition(CameraPosition.Cannon, new Vector3());
    }

    public void SetCameraPosition(CameraPosition position, Vector3 cameraDirection)
    {
        var camera = _cameraTransform.gameObject;
        var followEntity = camera.GetComponent<FollowEntity>();

        Debug.Log(followEntity);

        switch (position)
        {
            case CameraPosition.Cannon:

                if (followEntity != null)
                {
                    Destroy(followEntity);
                }

                _cameraTransform.parent = GameManager.GetCannonTransform();
                _cameraTransform.localPosition = new Vector3(0.6f, 1.3f, 0f);
                break;
            
            case CameraPosition.Cannonball:

                if (followEntity == null)
                {
                    followEntity = camera.AddComponent<FollowEntity>();
                }

                _cameraTransform.parent = null;

                followEntity.entityToFollow = _cannonballEntityQuery.GetSingletonEntity();
                followEntity.offset = new float3(-cameraDirection.x, -cameraDirection.y, -cameraDirection.z);
                Debug.Log($"cameraDirection: {cameraDirection} offset: {followEntity.offset}");
                break;
            
            default:
                _cameraTransform.position = Vector3.zero;
                break;
        }
    }
}
