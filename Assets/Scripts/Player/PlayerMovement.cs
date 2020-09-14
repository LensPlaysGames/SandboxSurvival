﻿using UnityEngine;

namespace U_Grow
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PlayerStats))]
    public class PlayerMovement : MonoBehaviour
    {
        public InputManager inputManager;
        public PlayerStats stats;

        Rigidbody2D rb;
        Animator anim;

        private Transform grounded;

        [SerializeField]
        [Range(5f, 25f)]
        private float runSpeed, walkSpeed;
        [SerializeField]
        private float totalStamina;
        public Color darkened, darker;
        [SerializeField]
        [Range(1f, 20f)]
        private float defaultJumpForce, lesserJumpForce;
        [SerializeField]
        [Range(2f, 20f)]
        private float fallMultiplier;
        [SerializeField]
        [Range(0f, 18f)]
        private float minVertSpeed;
        [SerializeField]
        private float maxFallSpeed;
        [SerializeField]
        [Range(0.01f, 10f)]
        private float groundedRadius;
        [SerializeField]
        private LayerMask ground;
        [SerializeField]
        private float staminaRegainTime;

        private float staminaRegain;
        private Vector3 input;
        private float speed, jumpForce, stamina;

        private Vector3 cachedPos;

        private void Awake()
        {
            inputManager = new InputManager();

            stats = GetComponent<PlayerStats>();
        }

        void OnEnable()
        {
            inputManager.Player.Enable();
        }
        void OnDisable()
        {
            inputManager.Player.Disable();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            grounded = transform.Find("GroundCheck");

            stamina = totalStamina;
        }
        private void Update()
        {
            GetInput(); // Get Data
        }
        private void FixedUpdate()
        {
            MovePlayer(); // Do Physics
            MultiplyFall(); // Do Physics

            Animate(); // Update Visuals
        }



        public void GetInput()
        {
            input.x = inputManager.Player.Movement.ReadValue<float>();

            #region Run/Walk && Stamina

            if (inputManager.Player.Sprint.ReadValue<float>() != 0)
            {

                staminaRegain = staminaRegainTime;

                if (stamina > 0)
                {
                    // Player Sprinting
                    stamina -= Time.deltaTime;
                    speed = runSpeed;
                    jumpForce = lesserJumpForce;
                    GetComponent<SpriteRenderer>().color = darkened;
                }
                else
                {
                    // Player Exhausted 
                    speed = walkSpeed / 2;
                    jumpForce = lesserJumpForce;
                    GetComponent<SpriteRenderer>().color = darker;
                }
            }
            else
            {
                // Player Walking
                if (stamina < totalStamina)
                {
                    if (staminaRegain <= 0)
                    {
                        stamina += Time.deltaTime;
                    }
                    else
                    {
                        staminaRegain -= Time.deltaTime;
                    }
                }
                else if (stamina >= totalStamina) { stamina = totalStamina; }

                speed = walkSpeed;
                jumpForce = defaultJumpForce;
                GetComponent<SpriteRenderer>().color = Color.white;
            }

            #endregion

            if (Physics2D.OverlapCircle(grounded.position, groundedRadius, ground)) { if (inputManager.Player.Jump.triggered) {Jump(); } } // If on ground and Press Jump, Jump()
            else // If in air and let go of jump, push downwards
            { 
                if (inputManager.Player.Jump.ReadValue<float>() == 0) 
                { 
                    rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime; 
                } 
            }

            if (inputManager.Player.Dash.triggered) { Dash(); }
        }
        public void MovePlayer()
        {
            transform.position += input * speed * stats.speedMultipler * Time.fixedDeltaTime;
        }
        public void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, Vector3.zero.y);
            rb.AddForce(Vector3.up * jumpForce * stats.jumpForceMultiplier, ForceMode2D.Impulse);
        }
        public void MultiplyFall()
        {
            if (rb.velocity.y < minVertSpeed)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && Input.GetAxisRaw("Jump") == 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
            }
        }
        public void Animate()
        {
            anim.SetFloat("playerSpeed", GetPlayerSpeed());
            anim.SetFloat("playerHorizontalInput", input.x);
        }

        #region Player Stat Accessors

        Vector3 lastPos = Vector3.zero;

        public float GetPlayerSpeed()
        {
            float speed = (transform.position - lastPos).magnitude;
            lastPos = transform.position;
            return speed;
        }
        public float GetPlayerStamina()
        {
            float sta = stamina / totalStamina;
            return sta;
        }

        #endregion

        public void Dash()
        {
            Debug.Log("Player Dashing");

            rb.velocity = new Vector3(rb.velocity.x, Vector3.zero.y);
            rb.AddForce(input * speed * stats.dashMultiplier, ForceMode2D.Impulse);
        }



        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.relativeVelocity.y >= maxFallSpeed)
            {
                UnityEngine.Debug.Log("Player Fell Too Hard!");
                GameReferences.uIHandler.SendNotif("Player Fell! Yeouch!", 10f, Color.red);
            }
        }
    }

}