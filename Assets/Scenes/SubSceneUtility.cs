using Unity.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SubSceneUtility
{
    /// <summary>
    /// Path must be relative to the Assets folder eg: 'Assets/NewSubScene.unity'.
    /// </summary>
    public static Scene CreateScene(string path = "Assets/Scenes/MenuSubScene.unity")
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        int indexOfName = path.LastIndexOf('/') + 1;
        string sceneName = path.Substring(indexOfName, path.Length - indexOfName);
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = sceneName;
        EditorSceneManager.SaveScene(scene, path);
        EditorSceneManager.SetActiveScene(activeScene);
        return scene;
    }
 
    public static void DeleteSubScene(GameObject subSceneGO)
    {

        SubScene subScene = subSceneGO.GetComponent<SubScene>();
        if (subScene == null || subScene.SceneAsset == null) { return; }
        string path = AssetDatabase.GetAssetPath(subScene.SceneAsset);
        AssetDatabase.DeleteAsset(path);
        MonoBehaviour.DestroyImmediate(subSceneGO);
    }
 
    public static GameObject CreateSubScene(Scene scene)
    {
        EditorSceneManager.SaveScene(scene);
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
        GameObject subSceneGO = new GameObject(scene.name);
        SubScene subScene = subSceneGO.AddComponent<SubScene>();
        subScene.SceneAsset = sceneAsset;
        EditorSceneManager.UnloadSceneAsync(scene);
        return subSceneGO;
    }
}