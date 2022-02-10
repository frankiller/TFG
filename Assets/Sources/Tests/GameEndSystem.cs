using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

public class GameEndSystem : SystemBase
{
    private SceneSystem _sceneSystem;
    private EntityQuery _subScenesEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        _subScenesEntityQuery = GetEntityQuery(ComponentType.ReadOnly<SceneReference>());

        RequireSingletonForUpdate<ReloadMenuSceneTag>();
        //RequireSingletonForUpdate<LoadGameSceneTag>();
    }

    protected override void OnUpdate()
    {
        //Hay que terminar primero el botón del menú desde el juego para poder asignar un botón a la vuelta hacia el menú principal
        //y que el nuevo estado aplique para ejecutar el sistema que guardará los resultados

        if (SceneManager.GetActiveScene().name != "Menu") return;

        using var subScenes = _subScenesEntityQuery.ToEntityArray(Allocator.Temp);

        Load(subScenes[1]);
        Unload(subScenes[0]);
        ResetGameManager();
    }

    private void Load(Entity subScenes)
    {
        var gameSceneGuid = GetComponent<SceneReference>(subScenes).SceneGUID;

        _sceneSystem.LoadSceneAsync(gameSceneGuid, new SceneSystem.LoadParameters {Flags = SceneLoadFlags.LoadAsGOScene});
    }

    private void Unload(Entity subScene)
    {
        _sceneSystem.UnloadScene(subScene);
        SceneManager.UnloadSceneAsync("GameSubScene");
    }

    private void ResetGameManager()
    {
        UnityEngine.Debug.Log("DEbuuuuuuuuug");
    }
}
