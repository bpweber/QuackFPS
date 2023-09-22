using UnityEngine;

public class MonoPlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    float speed;
    public float walkingSpeed = 4f;
    public float sprintingSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float jumpSpeed = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    public bool isGrounded;

    private void Start()
    {
        speed = sprintingSpeed;
    }

    void Update()
    {          


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = gravity;//-10;//-2;
        }

        speed = Input.GetKey(KeyCode.LeftShift) ? walkingSpeed : sprintingSpeed;

        float x = 0f;
        float z = 0f;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        //if(isGrounded)
        move = Vector3.ClampMagnitude(move, 1);

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * jumpSpeed * -gravity);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
