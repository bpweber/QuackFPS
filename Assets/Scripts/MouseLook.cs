using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{

    public static float mouseSensitivity = 1.50f;
    public static float zoomSens = 0.82f;
    public GameObject cameraHolder;
    public static float effectiveSens = mouseSensitivity;
    public Animator scopeAnim;

    private float xRotation = 0f;
    private Transform playerBody;
    private Animator anim;
    private WeaponSwitcher weaponSwitcher;
    private bool zoomed;


    public override void OnNetworkSpawn()
    {
        cameraHolder.SetActive(IsOwner);
        if (IsOwner)
        {
            playerBody = transform.root.transform;
            anim = cameraHolder.GetComponent<Animator>();
        }
        base.OnNetworkSpawn();
    }

    void Start()
    {    
        if(IsOwner)
        {
            weaponSwitcher = transform.root.GetComponent<WeaponSwitcher>();
        }
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


            if (Input.GetButtonDown("Fire2") && !weaponSwitcher.isSwitching)
            {
                anim.SetTrigger("ZoomIn");
                if(playerBody.GetComponent<WeaponSwitcher>().activeWep == 3)
                    scopeAnim.SetTrigger("Scope");
                effectiveSens = mouseSensitivity * zoomSens;
                zoomed = true;
            }
            else if(Input.GetButtonUp("Fire2") && zoomed)
            {
                anim.SetTrigger("ZoomOut");
                if (playerBody.GetComponent<WeaponSwitcher>().activeWep == 3)
                    scopeAnim.SetTrigger("UnScope");
                effectiveSens = mouseSensitivity;
                zoomed = false;
            }
        }
    }
}
