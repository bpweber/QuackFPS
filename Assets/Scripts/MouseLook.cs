using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{

    public static float mouseSensitivity = 1.50f;
    public Transform playerBody;
    public GameObject cameraHolder;
    float xRotation = 0f;

    public override void OnNetworkSpawn()
    {
        cameraHolder.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (!PauseMenu.GameIsPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * (mouseSensitivity / 2.5f);
            float mouseY = Input.GetAxisRaw("Mouse Y") * (mouseSensitivity / 2.5f);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
