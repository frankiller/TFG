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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public static void EndGame()
    {
        if (Instance == null) { return; }
        Instance._gameStateFsm = GameStateFSM.GameOver;
    }

    public static bool IsGameOver()
    {
        if (Instance == null) { return false; }

        return Instance._gameStateFsm == GameStateFSM.GameOver;
    }
}