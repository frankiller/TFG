using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float delay = 2.5f;
    [SerializeField] private UIDocument _menuUi;
    [SerializeField] private UIDocument _gameUi;
    [SerializeField] private UIDocument _pauseGameUi;
    [SerializeField] private UIDocument _finishedGameUi;
    [SerializeField] private CanvasGroup _canvasGroup;

    public UIDocument MenuUi => _menuUi;
    public UIDocument GameUi => _gameUi;
    public UIDocument PauseGameUi => _pauseGameUi;
    public UIDocument FinishedGameUi => _finishedGameUi;
    public CanvasGroup ScreenControlsCanvasGroup => _canvasGroup;

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
    }

    void Start()
    {
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
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator PlayGameRoutine()
    {
        Debug.Log("Starting to Play!");

        while (true)
        {
            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("EndGame");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
