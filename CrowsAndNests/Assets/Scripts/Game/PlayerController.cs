using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    /// <summary>
    /// Trida zajistujici ovladani hrace a vypocet jeho pohybu a chovani.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {

        /********************************************************************/
        // nastaveni pohybu (defaultni hodnoty pro mass = 10.0)
        [Header("Movement")]
        public float moveForce = 100.0f; /** sila pohybu */
        public float maxMoveSpeed = 6.0f; /** maximalni rychlost pohybu */
        public float groundDrag = 0.5f; /** faktor treni povrschu zeme */
        public float jumpForce = 190.0f; /** sila vyskoku */
        public float airMultiplier = 7.0f; /** faktor nasobeni sily, kdyz je hrac ve vzduchu */
        public float rotationSpeed = 3.0f; /** rychlost rotace hrace */
        public float gravity = 40.0f; /** dodatecna gravitacni sila posobici na hrace (pro vyladeni rychlosti padani) */

        /********************************************************************/
        // nastaveni ovladani
        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space; /** klavesa pro skok */
        public int mouseButtonAttack; /** tlacitko pro utok */

        // pro kontrolu kolize se zemi
        [Header("Ground Check")]
        public float playerHeight; /** vyska hrace */
        public LayerMask whatIsGround; /** vrstva ktera brana jako zeme */

        /********************************************************************/
        // reference na externi objekty
        [Header("References")]
        public Camera playerCamera; /** Kamera 3D osoby ktera sleduje hrace */
        public ParticleSystem dustParticle; /** Dust particly ktere se aktivouji pri dopadu hrace na zem */

        /********************************************************************/
        // zvuky
        [Header("Sounds")]
        public GameObject walkSoundObj; /** Zvuk chuze */
        public GameObject hitSoundObj;  /** Zvuk utoku */
        public GameObject jumpSoundObj; /** Zvuk skoku */


        /********************************************************************/
        // lokalni promenne
        private Quaternion targetRotation; /** Pro plynulou rotaci hrace po smeru pohybu */
        private Vector3 direction; /** Aktualni smer pohybu */

        private Rigidbody rb; /** Rigidbody hrace */
        private Animator animator; /** Animator controller */

        private float horizontalInput, verticalInput; /** Ovladani pohybu hrace */

        private bool inJump; /** True -> hrac prave skace */
        private Vector3 jumpDir; /** Smer skoku */
        private bool jumpBounce; /** True -> odraz od prekazky ve vzduchu, pri srazce z nejakym objektem */
        private bool inAttack; /** True -> hrace prave utoci */
        private bool grounded; /** True -> hrac je na zemi */
        private bool grounded_last; /** Predchozi stav "grounded */

        private AudioSource walkSound;
        private AudioSource hitSound;
        private AudioSource jumpSound;

        /// <summary>
        /// Inicializace promennych
        /// </summary>
        private void Start()
        {
            // nacte zvuky
            this.walkSound = this.walkSoundObj.GetComponent<AudioSource>();
            this.hitSound = this.hitSoundObj.GetComponent<AudioSource>();
            this.jumpSound = this.jumpSoundObj.GetComponent<AudioSource>();

            // skryje a uzamkne kurzor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // ziska referenci na rigid body hrace a freezne vypoce rotace
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            // init
            targetRotation = transform.rotation;
            inJump = false;
            jumpBounce = false;
            inAttack = false;
            grounded_last = false;

            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Update metoda. Vyhodnoceni ovladani hrace.
        /// </summary>
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
            if (Input.GetKey(jumpKey) && !inJump && grounded)
            {
                Jump();
            }
            // ovladani utoku
            if (Input.GetMouseButtonDown(mouseButtonAttack) && !inAttack)
            {
                Attack();
            }
            /**************************************************************************************************/

            // limitace rychlosti
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > maxMoveSpeed)
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
                if (inAttack)
                {
                    inAttack = false;
                    animator.SetBool("isAttacking", false);
                }
            }

            // animator (falling) zastaveni/spusteni
            animator.SetBool("isFalling", !grounded);

            // dopad na zem
            if (!grounded_last && grounded)
            {
                // animator (jumping) nastavi na false
                animator.SetBool("isJumping", false);
                // aktivace dust particlu
                if (dustParticle != null)
                {
                    dustParticle.Play();
                }
            }

            // predhozi stav "grounded"
            grounded_last = grounded;
        }

        /// <summary>
        /// Pro konstatni vypocet fyziky hrace
        /// </summary>
        private void FixedUpdate()
        {
            // gravitace
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

            // pokud utoci -> zastavi pohyb hrace
            if (inAttack)
            {
                return;
            }

            // pokud je ve vzduchu -> neni mozne ovladat pohyb letu ve vzduchu
            if (jumpDir != Vector3.zero)
            {
                if (!grounded && inJump)
                {
                    // pohyb ve vzduchu (smer kterym se hrac diva)
                    if (jumpBounce)
                    {
                        // zpetny odraz od prekazky (pustupny)
                        rb.AddForce(-jumpDir * 0.2f * moveForce * airMultiplier * 10f / 10, ForceMode.Force);
                    }
                    else
                    {
                        // sila pohybu ve vzduchu vpred
                        rb.AddForce(jumpDir * moveForce * airMultiplier * 10f, ForceMode.Force);
                    }
                    return;
                }
            }

            // vypocita smer pohybu
            direction = new Vector3(horizontalInput, 0, verticalInput);
            direction = playerCamera.transform.TransformDirection(direction);
            direction.y = 0;
            direction = direction.normalized;

            // animator (walking)
            animator.SetBool("isMoving", direction != Vector3.zero);

            // zvuk pohybu
            if(direction == Vector3.zero || inJump) {
                if(this.walkSound.isPlaying) {
                    this.walkSound.Stop();
                }
            } else {
                if(!this.walkSound.isPlaying) {
                    this.walkSound.Play();
                }
            }

            // otaceni modelu hrace ve smeru pohybu
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // pohyb kdyz je hrace na zemi
            rb.AddForce(direction.normalized * moveForce * 10f, ForceMode.Force);
        }

        /// <summary>
        /// Detekce kolizi. Urceno pro detekce kolizi se zemi.
        /// </summary>
        /// <param name="collision">Collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            // doslo ke kolizi z jim objektem -> pokud je hrac ve vzduchu obrati jeho vektor pohybu
            if (inJump)
            {
                rb.AddForce(transform.up * 15, ForceMode.Impulse);
                jumpBounce = true;
            }
            // pokud koliduje se zemi "inJump" = false -> muze skakat znovu
            if (Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround))
            {
                inJump = false;
            }
        }

        /// <summary>
        /// Funkce pro vyskok hrace.
        /// </summary>
        private void Jump()
        {
            // animator (jumping)
            animator.SetBool("isJumping", true);

            // propt set
            inJump = true;
            jumpBounce = false;

            // impulse + reset
            if (verticalInput != 0.0 || horizontalInput != 0.0)
            {
                jumpDir = transform.forward;
            }
            else
            {
                jumpDir = Vector3.zero;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            // zvuk
            this.jumpSound.Play();
        }

        /// <summary>
        /// Funkce pro utok hrace.
        /// </summary>
        private void Attack()
        {
            // animator utok
            animator.SetBool("isAttacking", true);
            inAttack = true;

            // zvuk
            this.hitSound.Play();
        }

    }

}
