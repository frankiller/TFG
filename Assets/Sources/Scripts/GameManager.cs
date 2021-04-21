using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Starting,
    Playing,
    Over
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float delay = 2f;

    private Transform _cannon;
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

        _cannon = FindObjectOfType<CannonManager>().transform;
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
    }

    private IEnumerator StartGameRoutine()
    {
        CannonManager.EnableCannon(true);

        //Instanciar primera isla y cañon

        yield return new WaitForSeconds(delay);
        _gameState = GameState.Playing;
    }

    private IEnumerator PlayGameRoutine()
    {
        while (_gameState == GameState.Playing)
        {
            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(delay);

        _gameState = GameState.Ready;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static Vector3 GetCannonPosition()
    {
        if (Instance == null) { return Vector3.zero; }

        return (Instance._cannon != null) ? GameManager.Instance._cannon.position : Vector3.zero;
    }

    public static void EndGame()
    {
        if (Instance == null) { return; }

        CannonManager.EnableCannon(false);
        Instance._gameState = GameState.Over;
    }

    public static bool IsGameOver()
    {
        if (Instance == null) { return false; }

        return Instance._gameState == GameState.Over;
    }
}
