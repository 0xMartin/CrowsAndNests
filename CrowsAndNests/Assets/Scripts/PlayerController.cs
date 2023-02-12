using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    /********************************************************************/
    // nastaveni pohybu (defaultni hodnoty pro mass = 10.0)
    [Header("Movement")]
    public float moveForce = 100.0f;
    public float maxMoveSpeed = 6.0f;
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
    public int mouseButtonAttack;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    /********************************************************************/
    // reference na externi objekty
    [Header("References")]
    public Camera camera; /** Kamera 3D osoby ktera sleduje hrace */
    public ParticleSystem dustParticle; /** Dust particly ktere se aktivouji pri dopadu hrace na zem */
    
    /********************************************************************/
    // lokalni promenne
    private Quaternion targetRotation;
    private Vector3 direction;

    private Rigidbody rb;
    private Animator animator;

    private float horizontalInput, verticalInput;

    private bool readyToJump;
    private bool inAttack;

    private bool grounded;
    private bool grounded_last;

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
        inAttack = false;
        grounded_last = false;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        /**************************************************************************************************/
        // OVLADANI
        // cteni vstupu uzivatele pro ovladani pohybu hrace
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        // ovladani skoku
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            Jump();
        }
        // ovladani utoku
        if(Input.GetMouseButtonDown(mouseButtonAttack)) {
            Attack();
        }
        /**************************************************************************************************/

        // limitace rychlosti
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > maxMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        // rigid body drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // zastaveni utoku "mode/animace"
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Attack")
        {
            if(inAttack) {
                inAttack = false;
                animator.SetBool("isAttacking", false); 
            }        
        }    

        // animator (falling) zastaveni/spusteni
        animator.SetBool("isFalling", !grounded);

        // dopad na zem
        if(!grounded_last && grounded) {
            // animator (jumping) nastavi na false
            animator.SetBool("isJumping", false);
            // aktivace dust particlu
            if(dustParticle != null) {
                dustParticle.Play();
            }
        }    

        // predhozi stav "grounded"
        grounded_last = grounded;
    }

    private void FixedUpdate()
    {
        // gravitace
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        // pokud utoci -> zastavi pohyb hrace
        if(inAttack) {
            return;
        }

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

    private void Attack() {
        // animator utok
        animator.SetBool("isAttacking", true); 
        inAttack = true;
    }

}
