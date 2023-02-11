using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    /********************************************************************/
    // nastaveni pohybu (defaultni hodnoty pro mass = 10.0)
    [Header("Movement")]
    public float moveForce = 100.0f;
    public float maxMoveSpeed = 8.0f;
    public float groundDrag = 0.5f;
    public float jumpForce = 190.0f;
    public float jumpCooldown = 1.2f;
    public float airMultiplier = 7.0f;
    public float rotationSpeed = 4.0f;
    public float gravity = 40.0f;

    /********************************************************************/
    // nastaveni ovladani
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    /********************************************************************/
    // reference na externi objekty
    [Header("References")]
    public Camera camera; /** Kamera 3D osoby ktera sleduje hrace */
    
    /********************************************************************/
    // lokalni promenne
    private Quaternion targetRotation;
    private bool readyToJump;
    private bool grounded;
    private bool grounded_last;
    private float horizontalInput, verticalInput;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 direction;

    private void Start()
    {
        // skryje a uzamkne kurzor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ziska referenci na rigid body hrace a freezne vypoce rotace
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // init
        targetRotation = transform.rotation;
        readyToJump = true;
        grounded_last = false;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        // cteni vstupu uzivatele pro ovladani hrace
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // skok
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            Jump();
        }

        // limitace rychlosti
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > maxMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // animator (falling)
        animator.SetBool("isFalling", !grounded);

        // animator (jumping) set to false
        if(!grounded_last && grounded) {
            Debug.Log("isJumping set to false");
            animator.SetBool("isJumping", false);
        }    

        grounded_last = grounded;
    }

    private void FixedUpdate()
    {
        // gravitace
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        // pokud je ve vzduchu -> neni mozne ovladat pohyb letu ve vzduchu
        if(!grounded) {
            // pohyb ve vzduchu (smer v okamziku vyskoku)
            if(direction != null) {
                rb.AddForce(direction.normalized * moveForce * airMultiplier * 10f, ForceMode.Force);
            }
            return;
        }

        // vypocita smer pohybu
        direction = new Vector3(horizontalInput, 0, verticalInput);
        direction = camera.transform.TransformDirection(direction);
        direction.y = 0;
        direction = direction.normalized;

        // animator (walking)
        animator.SetBool("isMoving", direction != Vector3.zero);

        // otaceni modelu hrace ve smeru pohybu
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // pohyb kdyz je hrace na zemi
        rb.AddForce(direction.normalized * moveForce * 10f, ForceMode.Force);
    }

    private void Jump()
    {
        // animator (jumping)
        animator.SetBool("isJumping", true);
        readyToJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

}
