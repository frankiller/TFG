using Unity.Entities;

public class CannonballManagerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimulationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.
            GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<CannonballTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        var gameManagerEntity = GetSingletonEntity<GameManagerTag>();
        var cannonballEntity = GetSingletonEntity<CannonballTag>();

        if (HasComponent<CannonballHitOnIslandTag>(gameManagerEntity))
        {
            ecb.DestroyEntity(cannonballEntity);
            ecb.RemoveComponent<CannonballHitOnIslandTag>(gameManagerEntity);
        }
        else if (HasComponent<CannonballMisshitTag>(gameManagerEntity))
        {
            ecb.DestroyEntity(cannonballEntity);
            ecb.RemoveComponent<CannonballMisshitTag>(gameManagerEntity);
        }

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
