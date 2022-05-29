using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CannonMuzzleSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsSystem;

    private float3 _muzzlePosition;
    private float3 _muzzleDirection;
    private quaternion _muzzleRotation;
    private float _impulseForce;
    private float _targetDetectionDistance;

    private Entity _cannonballPrefab;
    private Entity _gameManagerEntity;

    protected override void OnCreate()
    {
        base.OnCreate();

        _buildPhysicsSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();

        RequireSingletonForUpdate<GetPlayerActionsTag>();
        RequireSingletonForUpdate<CannonballPrefabConversion>();
        RequireSingletonForUpdate<FireActionTag>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        _gameManagerEntity = GetSingletonEntity<GameManagerTag>();
        _cannonballPrefab = GetSingleton<CannonballPrefabConversion>().CannonballPrefab;
        _impulseForce = GetSingleton<CannonInputData>().ShootImpulseForce;
        _targetDetectionDistance = GetSingleton<CannonInputData>().TargetDetectionDistance;
    }

    protected override void OnUpdate()
    {
        UpdateMuzzlePositionData();

        FireCannonball();
    }

    private void UpdateMuzzlePositionData()
    {
        var cameraPositionData = GetComponent<LocalToWorld>(GetSingletonEntity<CameraTag>());
        _muzzlePosition = cameraPositionData.Position;
        _muzzleDirection = cameraPositionData.Forward;
        _muzzleRotation = cameraPositionData.Rotation;
    }

    private void FireCannonball()
    {
        var cannonBallEntity = EntityManager.Instantiate(_cannonballPrefab);

        EntityManager.AddComponent<CannonballTag>(cannonBallEntity);
        EntityManager.SetComponentData(cannonBallEntity, new Translation { Value = _muzzlePosition });
        EntityManager.SetComponentData(cannonBallEntity, new Rotation { Value = _muzzleRotation });
        EntityManager.AddComponentData(cannonBallEntity, new CannonShootData
        {
            Direction = _muzzleDirection,
            Force = _impulseForce
        });

        if (IsTargetCorrectAnswer())
        {
            var playerSessionScore = GetSingleton<PlayerGameplayData>();
            playerSessionScore.RemainingOperations--;
            playerSessionScore.TimeExpended = GetSingleton<Chronometer>().ElapsedTime;
            EntityManager.SetComponentData(_gameManagerEntity, playerSessionScore);

            EntityManager.AddComponentData(cannonBallEntity, new TrajectoryPredictionData { Value = PredictTrajectory() });
        }

        EntityManager.RemoveComponent<FireActionTag>(_gameManagerEntity);
    }

    private bool IsTargetCorrectAnswer()
    {
        var collisionWorld = _buildPhysicsSystem.PhysicsWorld.CollisionWorld;

        var input = new RaycastInput
        {
            Start = _muzzlePosition,
            End = _muzzlePosition + _muzzleDirection * _targetDetectionDistance,
            Filter = CollisionFilter.Default
        };

        return collisionWorld.CastRay(input, out var hit) &&
               EntityManager.HasComponent<IsCorrectTag>(_buildPhysicsSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity);
    }

    private float3 PredictTrajectory()
    {
        return TrajectoryPrediction.Predict(_muzzlePosition, _muzzleDirection * _impulseForce);
    }
}
