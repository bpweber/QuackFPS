using UnityEngine;

public class MonoMouseLook : MonoBehaviour
{

    public static float mouseSensitivity = 1.50f;

    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

            float mouseX = Input.GetAxisRaw("Mouse X") * (mouseSensitivity / 2.5f);
            float mouseY = Input.GetAxisRaw("Mouse Y") * (mouseSensitivity / 2.5f);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        
    }
}
