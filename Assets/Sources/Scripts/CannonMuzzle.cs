using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CannonMuzzle : MonoBehaviour
{
    public float ImpulseForce = 50f;

    public bool IsFireButtonPressed { get; set; }
    public bool IsShootAllowed { get; set; }

    [SerializeField] private TrajectoryPredictionNoECS _trajectoryPrediction;

    [Header("Specs")]
    
    [SerializeField] private Transform _muzzleTransform;

    [SerializeField] private GameObject _cannonballPrefab;

    //Aquí iría la declaración de los efectos de sonido con su respectivo [Header("Effects")]

    private EntityManager _entityManager;
    private Entity _cannonballEntityPrefab;

    private BlobAssetStore _blobAssetStore;

    private Vector3 _currentPosition;
    private Quaternion _currentQuaternion;
    private Vector3 _predictedPosition;

    [SerializeField] private CameraController _cameraController;
    
    protected virtual void Start()
    {
        var currentWorld = World.DefaultGameObjectInjectionWorld;

        _entityManager = currentWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(currentWorld, _blobAssetStore = new BlobAssetStore());
        _cannonballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cannonballPrefab, settings);

        _currentPosition = transform.position;
        _currentQuaternion = transform.rotation;

        _predictedPosition = Predict();
    }

    protected virtual void Update()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        if (_currentPosition != transform.transform.position && !GameManager.IsFireState())
        {
            _predictedPosition = Predict();
        }

        if (_currentQuaternion != transform.rotation && !GameManager.IsFireState())
        {
            _predictedPosition = Predict();
        }

        _currentQuaternion = transform.rotation;

        //if (IsShootAllowed && IsFireButtonPressed)
        //{
        //    FireCannonball();
        //}

        if (IsFireButtonPressed)
        {
            GameManager.StartFireState();
            FireCannonball();
            _cameraController.SetCameraPosition(CameraPosition.Cannonball, _muzzleTransform.forward - _muzzleTransform.position);
        }
    }

    protected void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }

    public virtual void FireCannonball()
    {
        var cannonBallEntity = _entityManager.Instantiate(_cannonballEntityPrefab);
        
        _entityManager.SetName(cannonBallEntity, $"Cannonball-{cannonBallEntity.Index}");
        _entityManager.SetComponentData(cannonBallEntity, new Translation { Value = _muzzleTransform.position});
        _entityManager.SetComponentData(cannonBallEntity, new Rotation { Value = _muzzleTransform.rotation });
        _entityManager.AddComponentData(cannonBallEntity, new CannonballTag());
        _entityManager.AddComponentData(cannonBallEntity, new CannonShootData
        {
            Direction = _muzzleTransform.forward,
            Force = ImpulseForce,
            PredictedPosition = _predictedPosition
        });
    }

    private Vector3 Predict()
    {
       return _trajectoryPrediction.Predict(_cannonballPrefab, _muzzleTransform.position, _muzzleTransform.forward * ImpulseForce);
    }
}
