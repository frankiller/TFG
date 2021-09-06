//using Unity.Entities;
//using Unity.Physics;
//using Unity.Physics.Systems;
//using Unity.Transforms;
//using UnityEngine;

//public class CannonMuzzle : MonoBehaviour
//{
//    public float ImpulseForce = 50f;
//    public LayerMask TargetLayerMask;
//    public GameObject CannonballPrefab;

//    public bool IsFireButtonPressed { get; set; }
//    public bool IsShootAllowed { get; set; }

//    private TrajectoryPredictionNoECS _trajectoryPrediction;

//    [Header("Specs")]
    
//    private Transform _muzzleTransform;
    
//    //Aquí iría la declaración de los efectos de sonido con su respectivo [Header("Effects")]

//    private EntityManager _entityManager;
//    private Entity _cannonballEntityPrefab;
//    private BuildPhysicsWorld _buildPhysicsSystem;

//    private BlobAssetStore _blobAssetStore;

//    private Vector3 _currentPosition;
//    private Quaternion _currentQuaternion;
//    private Vector3 _predictedPosition;

//    protected virtual void Awake()
//    {
//        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore = new BlobAssetStore());
//        _cannonballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(CannonballPrefab, settings);

//        Debug.Log("Muzzle Awake");
//    }

//    protected virtual void Start()
//    {
//        Debug.Log("Muzzle Start");

//        var currentWorld = World.DefaultGameObjectInjectionWorld;
        
//        _buildPhysicsSystem = currentWorld.GetOrCreateSystem<BuildPhysicsWorld>();

//        _entityManager = currentWorld.EntityManager;


//        //_currentPosition = CannonManager.GetCannonBarrelTranslation();
//        _currentQuaternion = CannonManager.GetCannonBarrelRotation();

//        _trajectoryPrediction = ScriptableObject.CreateInstance<TrajectoryPredictionNoECS>();
//        _muzzleTransform = Camera.main.transform;

//        _predictedPosition = Predict();
//    }

//    protected virtual void Update()
//    {
//        if (GameManager.IsGameOver())
//        {
//            return;
//        }

//        if (_currentPosition != transform.transform.position && !GameManager.IsFireState())
//        {
//            _predictedPosition = Predict();
//        }

//        if (_currentQuaternion != transform.rotation && !GameManager.IsFireState())
//        {
//            _predictedPosition = Predict();
//        }

//        _currentQuaternion = transform.rotation;

//        if (IsShootAllowed && IsFireButtonPressed)
//        {
//            FireCannonball();
//        }

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            Debug.Log("Muzzle Update");
//            GameManager.EndGame();
//        }

//        if (!IsFireButtonPressed) return;

//        GameManager.StartFireState();
//        FireCannonball();
//        GameManager.CameraController.SetCameraPosition(CameraPosition.Cannonball, _muzzleTransform.forward - _muzzleTransform.position);
//    }

//    protected void FixedUpdate()
//    {
//        _trajectoryPrediction.Simulate();
//    }

//    protected void OnDestroy()
//    {
//        _blobAssetStore.Dispose();
//    }

//    public virtual void FireCannonball()
//    {
//        var cannonBallEntity = _entityManager.Instantiate(_cannonballEntityPrefab);
        
//        _entityManager.SetName(cannonBallEntity, $"Cannonball-{cannonBallEntity.Index}");
//        _entityManager.SetComponentData(cannonBallEntity, new Translation { Value = _muzzleTransform.position});
//        _entityManager.SetComponentData(cannonBallEntity, new Rotation { Value = _muzzleTransform.rotation });
//        _entityManager.AddComponentData(cannonBallEntity, new CannonballTag());
//        _entityManager.AddComponentData(cannonBallEntity, new CannonShootData
//        {
//            Direction = _muzzleTransform.forward,
//            Force = ImpulseForce,
//            PredictedPosition = _predictedPosition
//        });

//        if (IsTargetCorrectAnswer())
//        {
//            _entityManager.AddComponentData(cannonBallEntity, new IsCorrect());
//        }
//    }

//    private bool IsTargetCorrectAnswer()
//    {
//        var collisionWorld = _buildPhysicsSystem.PhysicsWorld.CollisionWorld;

//        var input = new RaycastInput
//        {
//            Start = _muzzleTransform.position,
//            End = _muzzleTransform.forward * ImpulseForce,
//            Filter = new CollisionFilter
//            {
//                BelongsTo = ~0u,
//                CollidesWith = ~0u,
//                GroupIndex = 0
//            }
//        };

//        return collisionWorld.CastRay(input, out var hit) 
//               && _entityManager.HasComponent<IsCorrect>(_buildPhysicsSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity);
//    }

//    private Vector3 Predict()
//    {
//       return _trajectoryPrediction.Predict(CannonballPrefab, _muzzleTransform.position, _muzzleTransform.forward * ImpulseForce);
//    }
//}
