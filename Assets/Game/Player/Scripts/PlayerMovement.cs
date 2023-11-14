using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;
    public float sprintSpeed;
    public float airSpeed;
    public float jumpHeight;

    public static float gravity = -9.81f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;
    private Vector2 moveInput = Vector2.zero;

    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    public CharacterController cc;
    public Transform cam;

    public bool grounded;

    public bool recieveInput;

    PlayerInput playerInput;
    

    void Start()
    {
        recieveInput = true;
        if(cc == null) cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        //playerInput.actions["move"].Enable();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        print("moving at: " + moveInput);
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput.magnitude > 1) moveInput.Normalize();

        if(recieveInput)
        if (grounded)
        {
            moveDir = cc.transform.right * moveInput.x + cc.transform.forward * moveInput.y;
            moveDir *= Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        }
        else moveDir += (cc.transform.right * moveInput.x + cc.transform.forward * moveInput.y) * airSpeed * Time.deltaTime;

        if (grounded && velocity.y < 0) velocity.y = -2f;

        cc.Move(moveDir * Time.deltaTime);

        if (grounded && Input.GetButtonDown("Jump") && recieveInput)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    public void SetForce(Vector3 force)
    {
        moveDir = force;
    }

    public Vector3 GetForce()
    {
        return moveDir;
    }
}
