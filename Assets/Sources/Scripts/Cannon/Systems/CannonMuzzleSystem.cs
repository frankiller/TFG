using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine.InputSystem;
using Material = Unity.Physics.Material;
using SphereCollider = Unity.Physics.SphereCollider;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonMuzzleSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsSystem;
    private EntityManager _entityManager;

    private float3 _predictedTrajectory;

    private float3 _muzzlePosition;
    private float3 _muzzleDirection;
    private quaternion _muzzleRotation;
    private float _impulseForce;

    private float3 _currentPosition;
    private quaternion _currentRotation;
    private float3 _lastPosition = float3.zero;
    private quaternion _lastRotation = quaternion.identity;

    private EntityQuery _cannonballEntityQuery;
    private Entity _cannonballPrefab;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        
        _buildPhysicsSystem = currentWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        _entityManager = currentWorld.EntityManager;

        _cannonballEntityQuery = GetEntityQuery(ComponentType.ReadOnly<CannonballTag>());

        RequireSingletonForUpdate<GetPlayerActionsTag>();
        RequireSingletonForUpdate<CannonballPrefabConversion>(); 
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        _cannonballPrefab = GetSingleton<CannonballPrefabConversion>().CannonballPrefab;
        _impulseForce = GetSingleton<InputVariables>().ImpulseForce;
    }

    protected override void OnUpdate()
    {
        if (!_cannonballEntityQuery.IsEmptyIgnoreFilter) return;

        UpdateMuzzlePositionData();
        UpdateBarrelPositionData();

        if (math.all(_currentPosition != _lastPosition))
        {
            _predictedTrajectory = PredictTrajectory();
        }

        _lastPosition = _currentPosition;

        if (!_currentRotation.Equals(_lastRotation))
        {
            _predictedTrajectory = PredictTrajectory();
        }

        _lastRotation = _currentRotation;

        if (!GetSingleton<LeftMouseButtonData>().IsClicked) return;

        FireCannonball();
    }

    private void UpdateMuzzlePositionData()
    {
        var cameraPositionData = GetComponent<LocalToWorld>(GetSingletonEntity<CameraTag>());
        _muzzlePosition = cameraPositionData.Position;
        _muzzleDirection = cameraPositionData.Forward;
        _muzzleRotation = cameraPositionData.Rotation;
    }

    private void UpdateBarrelPositionData()
    {
        _currentPosition = CannonHelper.GetCannonBarrelPosition(_entityManager);
        _currentRotation = CannonHelper.GetCannonBarrelRotation(_entityManager);
    }

    private void FireCannonball()
    {
        var cannonBallEntity = _entityManager.Instantiate(_cannonballPrefab);

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

        if (!IsTargetCorrectAnswer()) return;

        _entityManager.AddComponentData(cannonBallEntity, new PositionPredictionData { Value = _predictedTrajectory });
        _entityManager.SetComponentData(GetSingletonEntity<MenuManagerTag>(), new SuccessLabelData { IsVisible = true });

        var score = GetSingleton<PlayerSessionScoreData>();
        score.Score++;
        _entityManager.SetComponentData(GetSingletonEntity<GameManagerTag>(), score);
        
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
        return new TrajectoryPredictionNoECS().Predict(_muzzlePosition, _muzzleDirection * _impulseForce);
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(CannonMuzzleSystem))]
public class ManagePlayerInput : SystemBase
{
    protected override void OnUpdate()
    {
        var wasPressed = Mouse.current.leftButton.wasPressedThisFrame;

        Entities.ForEach((ref LeftMouseButtonData leftMouseButton) =>
        {
            leftMouseButton.IsClicked = wasPressed;
        }).Run();
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class FireCannonSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        
    }
}
