using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Material = Unity.Physics.Material;
using SphereCollider = Unity.Physics.SphereCollider;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonMuzzleSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsSystem;
    private EntityManager _entityManager;
    private BlobAssetStore _blobAssetStore;
    
    private TrajectoryPredictionNoECS _trajectoryPrediction;
    private float3 _predictedTrajectory;
    
    private GameObject _cannonballPrefab;
    private Entity _cannonballEntityPrefab;
    
    private float3 _muzzlePosition;
    private float3 _muzzleDirection;
    private quaternion _muzzleRotation;
    private float _impulseForce;

    private float3 _currentPosition;
    private quaternion _currentRotation;
    private float3 _lastPosition = float3.zero;
    private quaternion _lastRotation = quaternion.identity;

    private bool _isFireButtonPressed;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        
        _buildPhysicsSystem = currentWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        _entityManager = currentWorld.EntityManager;

        _cannonballPrefab = Resources.Load("Prefabs/CannonballNoECS") as GameObject;

        var settings = GameObjectConversionSettings.FromWorld(currentWorld, _blobAssetStore = new BlobAssetStore());
        _cannonballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cannonballPrefab, settings);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        _trajectoryPrediction = ScriptableObject.CreateInstance<TrajectoryPredictionNoECS>();
        _impulseForce = GetComponent<InputVariables>(GetSingletonEntity<InputVariables>()).ImpulseForce;
    }

    protected override void OnUpdate()
    {
        if (GameManager.IsGameOver() || !GameManager.IsPlayState()) { return; }

        UpdateMuzzlePositionData();

        UpdateBarrelPositionData();

        if (math.all(_currentPosition != _lastPosition) && !GameManager.IsFireState())
        {
            _predictedTrajectory = PredictTrajectory();
        }

        _lastPosition = _currentPosition;

        if (!_currentRotation.Equals(_lastRotation) && !GameManager.IsFireState())
        {
            _predictedTrajectory = PredictTrajectory();
        }

        _lastRotation = _currentRotation;

        //Se puede sacar de forma que en otro sistema se detecte la pulsación
        Entities.WithoutBurst().ForEach((in InputData inputData) =>
        {
            _isFireButtonPressed = Input.GetMouseButtonDown(inputData.LeftMouseButton);
        }).Run();

        if (!_isFireButtonPressed || GameManager.IsFireState()) return;

        GameManager.StartFireState();
        FireCannonball();
        GameManager.CameraController.SetCameraPosition(CameraPositionNoEcs.Cannonball, _muzzleDirection);
    }

    private void UpdateMuzzlePositionData()
    {
        var cameraPositionData =
            _entityManager.GetComponentData<LocalToWorld>(_entityManager.CreateEntityQuery(typeof(CameraTag))
                .GetSingletonEntity());
        _muzzlePosition = cameraPositionData.Position;
        _muzzleDirection = cameraPositionData.Forward;
        _muzzleRotation = cameraPositionData.Rotation;
    }

    private void UpdateBarrelPositionData()
    {
        _currentPosition = CannonManager.GetCannonBarrelPosition();
        _currentRotation = CannonManager.GetCannonBarrelRotation();
    }

    private void FireCannonball()
    {
        var cannonBallEntity = _entityManager.Instantiate(_cannonballEntityPrefab);

        _entityManager.SetName(cannonBallEntity, $"Cannonball-{cannonBallEntity.Index}");
        _entityManager.SetComponentData(cannonBallEntity, new Translation { Value = _muzzlePosition });
        _entityManager.SetComponentData(cannonBallEntity, new Rotation { Value = _muzzleRotation });
        _entityManager.AddComponentData(cannonBallEntity, new CannonballTag());
        _entityManager.AddComponentData(cannonBallEntity, new CannonShootData
        {
            Direction = _muzzleDirection,
            Force = _impulseForce
        });

        _entityManager.AddComponentData(cannonBallEntity, new PhysicsCollider
        {
            Value = SphereCollider.Create(new SphereGeometry
            {
                Center = 0f,
                Radius = 0.5f
            }, CollisionFilter.Default, new Material
            {
                CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents
            })
        });

        if (IsTargetCorrectAnswer())
        {
            _entityManager.AddComponentData(cannonBallEntity, new PositionPredictionData { PredictedPosition = _predictedTrajectory });
        }
    }

    private bool IsTargetCorrectAnswer()
    {
        var collisionWorld = _buildPhysicsSystem.PhysicsWorld.CollisionWorld;

        var input = new RaycastInput
        {
            Start = _muzzlePosition,
            End = _muzzlePosition + _muzzleDirection * _impulseForce,
            Filter = CollisionFilter.Default
        };

        return collisionWorld.CastRay(input, out var hit) &&
               _entityManager.HasComponent<IsCorrectTag>(_buildPhysicsSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity);
    }

    private float3 PredictTrajectory()
    {
        return _trajectoryPrediction.Predict(_muzzlePosition, _muzzleDirection * _impulseForce);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _blobAssetStore.Dispose();
    }
}
