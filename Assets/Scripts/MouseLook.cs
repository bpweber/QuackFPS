using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{

    public static float mouseSensitivity = 1.50f;
    public static float zoomSens = 0.5f;
    public Transform playerBody;
    public GameObject cameraHolder;
    public float effectiveSens = mouseSensitivity;

    float xRotation = 0f;

    public Camera playerCam;
    private Animator anim;


    public override void OnNetworkSpawn()
    {
        cameraHolder.SetActive(IsOwner);
        if(IsOwner)
            anim = cameraHolder.GetComponent<Animator>();
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
            float mouseX = Input.GetAxisRaw("Mouse X") * (effectiveSens / 2.5f);
            float mouseY = Input.GetAxisRaw("Mouse Y") * (effectiveSens / 2.5f);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);


            if (Input.GetButtonDown("Fire2"))
            {
                anim.SetTrigger("ZoomIn");
                effectiveSens = mouseSensitivity * zoomSens;
            }
            else if(Input.GetButtonUp("Fire2"))
            {
                anim.SetTrigger("ZoomOut");
                effectiveSens = mouseSensitivity;
            }
        }
    }
}
