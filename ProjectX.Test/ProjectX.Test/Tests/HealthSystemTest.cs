using Shouldly;

class HealthSystemTest : EntitasTests
{
    void when_executing()
    {
        GameEntity e = null;

        before = () =>
        {
            Setup();
            var system = new HealthSystem(Contexts);
            Systems.Add(system);
            e = Contexts.game.CreateEntity();
        };

        it["flags entities as destroyed when health is 0"] = () =>
        {
            e.AddHealth(0);
            Systems.Execute();
            e.isDestroyed.ShouldBeTrue();
        };

        it["flags entities as destroyed when health is less than 0"] = () =>
        {
            e.AddHealth(-1);
            Systems.Execute();
            e.isDestroyed.ShouldBeTrue();
        };

        it["doesn't flags entities as destroyed when health is greater than 0"] = () =>
        {
            e.AddHealth(1);
            Systems.Execute();
            e.isDestroyed.ShouldBeFalse();
        };
    }
}