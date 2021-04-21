using Shouldly;

class DestroyEntitySystemTest : EntitasTests
{
    void when_executing()
    {
        GameEntity e = null;

        before = () =>
        {
            Setup();
            var system = new DestroyEntitySystem(Contexts);
            Systems.Add(system);
            e = Contexts.game.CreateEntity();
        };

        it["destroys entities flagged as destroyed"] = () =>
        {
            e.isDestroyed = true;
            Systems.Execute();
            Contexts.game.count.ShouldBe(0);

        };

        it["doesn't destroy entities not flagged as destroyed"] = () =>
        {
            Systems.Execute();
            Contexts.game.count.ShouldBe(1);
        };
    }
}