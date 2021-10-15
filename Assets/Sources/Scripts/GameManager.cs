using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStateFSM
{
    Ready,
    Starting,
    Playing,
    Shooting,
    Landing,
    GameOver
}

[RequireComponent(typeof(CameraController))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static CameraController CameraController;

    [SerializeField] private float delay = 2f;

    private GameStateFSM _gameStateFsm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

        CameraController = GetComponent<CameraController>();

        _gameStateFsm = GameStateFSM.Ready;
    }

    void Start()
    {
        _gameStateFsm = GameStateFSM.Starting;
        StartCoroutine(MainGameLoopRoutine());
    }

    private IEnumerator MainGameLoopRoutine()
    {
        yield return StartGameRoutine();
        yield return PlayGameRoutine();
        yield return EndGameRoutine();
    }

    private IEnumerator StartGameRoutine()
    {
        //Instanciar primera isla
        
        yield return new WaitForSeconds(delay);
        
        _gameStateFsm = GameStateFSM.Playing;
    }

    private IEnumerator PlayGameRoutine()
    {
        Debug.Log("Starting to Play!");

        while (_gameStateFsm == GameStateFSM.Playing || _gameStateFsm == GameStateFSM.Shooting)
        {
            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(delay);

        _gameStateFsm = GameStateFSM.Ready;

        Debug.Log("EndGame");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //public static Vector3 GetCannonPosition()
    //{
    //    if (Instance == null) { return Vector3.zero; }

    //    return Instance._cannon != null ? GameManager.Instance._cannon.position : Vector3.zero;
    //}

    //public static Transform GetCannonTransform()
    //{
    //    if (Instance == null) { return new RectTransform(); }

    //    return Instance._cannon != null ? Instance._cannon : new RectTransform();
    //}

    public static bool IsStartingState()
    {
        if (Instance == null) { return false; }

        return Instance._gameStateFsm == GameStateFSM.Starting;
    }

    public static void StartPlayState()
    {
        if (Instance == null) { return; }

        Instance._gameStateFsm = GameStateFSM.Playing;
    }

    public static bool IsPlayState()
    {
        if (Instance == null) { return false; }

        return Instance._gameStateFsm == GameStateFSM.Playing;
    }

    public static void StartFireState()
    {
        if (Instance == null) { return; }

        Instance._gameStateFsm = GameStateFSM.Shooting;
    }

    public static bool IsFireState()
    {
        if (Instance == null) { return false; }

        return Instance._gameStateFsm == GameStateFSM.Shooting;
    }

    public static void StartLandingState()
    {
        if (Instance == null) { return; }

        Instance._gameStateFsm = GameStateFSM.Landing;
    }

    public static bool IsLandingState()
    {
        if (Instance == null) { return false; }

        return Instance._gameStateFsm == GameStateFSM.Landing;
    }

    public static void EndGame()
    {
        if (Instance == null) { return; }

        CameraController.ResetCameraPosition();
        CannonManager.EnableCannon(false);
        Instance._gameStateFsm = GameStateFSM.GameOver;
    }

    public static bool IsGameOver()
    {
        if (Instance == null) { return false; }

        return Instance._gameStateFsm == GameStateFSM.GameOver;
    }
}
