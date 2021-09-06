using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonMuzzleSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsSystem;
    private EntityManager _entityManager;

    private TrajectoryPredictionNoECS _trajectoryPrediction;
    private float3 _predictedTrajectory;
    private Transform _muzzleTransform;
    private GameObject _cannonballPrefab;
    private Entity _cannonballEntityPrefab;
    private BlobAssetStore _blobAssetStore;

    private float3 _currentPosition;
    private quaternion _currentRotation;
    private float3 _lastPosition = float3.zero;
    private quaternion _lastRotation = quaternion.identity;
    private float3 _lastPrediction = float3.zero;

    private bool isFireButtonPressed = false;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        
        _buildPhysicsSystem = currentWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        _entityManager = currentWorld.EntityManager;

        _trajectoryPrediction = ScriptableObject.CreateInstance<TrajectoryPredictionNoECS>();
        _muzzleTransform = Camera.main.transform;
        _cannonballPrefab = Resources.Load("Prefabs/CannonballNoECS") as GameObject;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore = new BlobAssetStore());
        _cannonballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cannonballPrefab, settings);
    }

    protected override void OnUpdate()
    {
        if (GameManager.IsGameOver()) { return; }

        _currentPosition = CannonManager.GetCannonBarrelPosition();
        _currentRotation = CannonManager.GetCannonBarrelRotation();

        if (math.all(_currentPosition != _lastPosition) && !GameManager.IsFireState())
        {
            _predictedTrajectory = PredictTrajectory();
            SimulatePhysics();
        }

        _lastPosition = _currentPosition;

        if (!_currentRotation.Equals(_lastRotation) && !GameManager.IsFireState())
        {
            _predictedTrajectory = PredictTrajectory();
            SimulatePhysics();
        }

        _lastRotation = _currentRotation;

        //_predictedTrajectory = _trajectoryPrediction.Predict(_cannonballPrefab, _muzzleTransform.position,_muzzleTransform.forward * 50f);
        

        //if(math.all(_lastPrediction != _predictedTrajectory))
        //    Debug.Log("Predicted position " + _predictedTrajectory);

        //_lastPrediction = _predictedTrajectory;

        //Se puede sacar de forma que en otro sistema se detecte la pulsación
        Entities.WithoutBurst().ForEach((in InputData inputData) =>
        {
            isFireButtonPressed = Input.GetMouseButtonDown(inputData.LeftMouseButton);
        }).Run();

        if (!isFireButtonPressed || GameManager.IsFireState()) return;

        GameManager.StartFireState();
        FireCannonball();
        GameManager.CameraController.SetCameraPosition(CameraPosition.Cannonball, _muzzleTransform.forward, _muzzleTransform.eulerAngles);
    }

    private void FireCannonball()
    {
        var cannonBallEntity = _entityManager.Instantiate(_cannonballEntityPrefab);

        _entityManager.SetName(cannonBallEntity, $"Cannonball-{cannonBallEntity.Index}");
        _entityManager.SetComponentData(cannonBallEntity, new Translation { Value = _muzzleTransform.position});
        _entityManager.SetComponentData(cannonBallEntity, new Rotation { Value = _muzzleTransform.rotation });
        _entityManager.AddComponentData(cannonBallEntity, new CannonballTag());
        _entityManager.AddComponentData(cannonBallEntity, new CannonShootData
        {
            Direction = _muzzleTransform.forward,
            Force = 100f, //Aplicar ImpulseForce desde InputVariables
            PredictedPosition = _predictedTrajectory
        });

        if (IsTargetCorrectAnswer())
        {
            _entityManager.AddComponentData(cannonBallEntity, new IsCorrect());
        }
    }

    private bool IsTargetCorrectAnswer()
    {
        var collisionWorld = _buildPhysicsSystem.PhysicsWorld.CollisionWorld;

        var input = new RaycastInput
        {
            Start = _muzzleTransform.position,
            End = _muzzleTransform.forward * 100f, //Aplicar ImpulseForce desde InputVariables
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        return collisionWorld.CastRay(input, out var hit)
               && _entityManager.HasComponent<IsCorrect>(_buildPhysicsSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity);
    }

    private float3 PredictTrajectory()
    {
        return _trajectoryPrediction.Predict(_cannonballPrefab, _muzzleTransform.position, _muzzleTransform.forward * 100f);
    }

    private void SimulatePhysics()
    {
        _trajectoryPrediction.Simulate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _blobAssetStore.Dispose();
    }
}
