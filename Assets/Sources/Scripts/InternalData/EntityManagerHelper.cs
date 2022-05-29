using Unity.Entities;

public static class EntityManagerHelper
{
    public static EntityManager GetEntityManager()
    {
        return World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
    }

    public static Entity GetGameManagerEntity()
    {
        var gameManagerEntityQuery = GetEntityManager().CreateEntityQuery(ComponentType.ReadOnly<GameManagerTag>());

        return gameManagerEntityQuery.IsEmpty ? Entity.Null : gameManagerEntityQuery.GetSingletonEntity();
    }

    public static Entity GetMenuManagerEntity()
    {
        var menuManagerEntityQuery = GetEntityManager().CreateEntityQuery(ComponentType.ReadOnly<MenuManagerTag>());

        return menuManagerEntityQuery.IsEmpty ? Entity.Null : menuManagerEntityQuery.GetSingletonEntity();
    }
}
