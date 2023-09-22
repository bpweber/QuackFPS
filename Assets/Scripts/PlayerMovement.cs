using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float walkingSpeed = 4f;
    public float sprintingSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float jumpSpeed = 2f;
    public float groundDistance = 0.4f;
    public bool isGrounded;

    private float speed;
    private Vector3 velocity;

    private void Start()
    {
        speed = sprintingSpeed;
    }

    void Update()
    {          

        if(!IsOwner) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = gravity;

        speed = Input.GetKey(KeyCode.LeftShift) ? walkingSpeed : sprintingSpeed;

        float x = 0f;
        float z = 0f;

        if (!PauseMenu.GameIsPaused)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        Vector3 move = transform.right * x + transform.forward * z;

        move = Vector3.ClampMagnitude(move, 1);

        controller.Move(move * speed * Time.deltaTime);

        if (!PauseMenu.GameIsPaused && Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * jumpSpeed * -gravity);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
