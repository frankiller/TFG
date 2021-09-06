using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Starting,
    Playing,
    Shooting,
    Over
}

[RequireComponent(typeof(CameraController))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static CameraController CameraController;

    [SerializeField] private float delay = 2f;

    //private Transform _cannon;
    //Referencia al script de spawneo de bolas

    private GameState _gameState;

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

        //_cannon = FindObjectOfType<CannonManager>().transform;
        //Instancia al script de spawneo de bolas

        _gameState = GameState.Ready;
    }

    void Start()
    {
        _gameState = GameState.Starting;
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
        
        _gameState = GameState.Playing;
    }

    private IEnumerator PlayGameRoutine()
    {
        Debug.Log("Starting to Play!");

        while (_gameState == GameState.Playing || _gameState == GameState.Shooting)
        {
            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(delay);

        _gameState = GameState.Ready;

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

        return Instance._gameState == GameState.Starting;
    }

    public static bool IsPlayState()
    {
        if (Instance == null) { return false; }

        return Instance._gameState == GameState.Playing;
    }

    public static void StartFireState()
    {
        if (Instance == null) { return; }

        Instance._gameState = GameState.Shooting;
    }

    public static bool IsFireState()
    {
        if (Instance == null) { return false; }

        return Instance._gameState == GameState.Shooting;
    }

    public static void EndGame()
    {
        if (Instance == null) { return; }

        CameraController.ResetCameraPosition();
        CannonManager.EnableCannon(false);
        Instance._gameState = GameState.Over;
    }

    public static bool IsGameOver()
    {
        if (Instance == null) { return false; }

        return Instance._gameState == GameState.Over;
    }
}
