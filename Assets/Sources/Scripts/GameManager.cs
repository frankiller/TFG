using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float delay = 2f;
    [SerializeField] private UIDocument _menuUI;
    [SerializeField] private UIDocument _gameUI;

    public UIDocument MenuUI
    {
        get => _menuUI;
        set => value = _menuUI;
    }

    public UIDocument GameUI
    {
        get => _gameUI;
        set => value = _gameUI;
    }


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