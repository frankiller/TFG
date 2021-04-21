using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShoot : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            var a = Quaternion.Euler(new Vector3(0, 0, 0));
            var b = Quaternion.Euler(new Vector3(50, 50, 50));

            //transform.rotation = Quaternion.Slerp(a, b, 0.5f).normalized;

            Debug.Log("Objective Locked");

            transform.position += Quaternion.Slerp(a, b, 0.5f).normalized.eulerAngles * (10f * Time.deltaTime);
        }
    }

}
