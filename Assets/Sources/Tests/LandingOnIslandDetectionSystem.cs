using Unity.Entities;

public class LandingOnIslandDetectionSystem : SystemBase
{
    private EntityManager _entityManager;
    private EntityQuery _cannonballEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        var currentWorld = World.DefaultGameObjectInjectionWorld;
        _entityManager = currentWorld.EntityManager;

        _cannonballEntityQuery = _entityManager.CreateEntityQuery(new ComponentType[] {typeof(CannonballTag)});

        RequireForUpdate(_cannonballEntityQuery);
    }

    protected override void OnUpdate()
    {
        if (!GameManager.IsFireState() || 
            _cannonballEntityQuery.CalculateEntityCount() == 0) return;


    }
}
