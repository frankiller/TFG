using UnityEngine;

public class CannonManager : MonoBehaviour
{
    public static string cannonTagName = "Cannon";

    [SerializeField] private Camera _sceneCamera;

    private CannonMover _cannonMover;
    private CannonMuzzle _cannonMuzzle;

    //Generar CannonInput si fuera necesario e introducir esta propiedad
    public bool IsFiring => Input.GetButtonDown("Fire1");

    private void Awake()
    {
        _cannonMover = GetComponent<CannonMover>();
        _cannonMuzzle = GetComponent<CannonMuzzle>();

        EnableCannon(true);
    }

    private void Update()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        _cannonMuzzle.IsFireButtonPressed = IsFiring;
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGameOver())
        {
            EnableCannon(false);
            return;
        }

        _cannonMover.AimAtMousePosition();
    }

    public static void EnableCannon(bool state)
    {
        GameObject.FindGameObjectWithTag(cannonTagName).SetActive(state);
    }
}
