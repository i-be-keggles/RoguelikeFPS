using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;
    public float sprintSpeed;
    public float airSpeed;
    public float jumpHeight;

    public static float gravity = -9.81f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;

    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    public CharacterController cc;
    public Transform cam;

    public bool grounded;

    void Start()
    {
        if(cc == null) cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (grounded)
        {
            moveDir = cc.transform.right * Input.GetAxis("Horizontal") + cc.transform.forward * Input.GetAxis("Vertical");
            moveDir *= Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        }
        else moveDir += (cc.transform.right * Input.GetAxis("Horizontal") + cc.transform.forward * Input.GetAxis("Vertical")) * airSpeed * Time.deltaTime;

        if (grounded && velocity.y < 0) velocity.y = -2f;

        cc.Move(moveDir * Time.deltaTime);

        if (grounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
