using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputManager inputManager;

    public Rigidbody2D rb;
    public Animator anim;
    public Transform grounded;

    public int level;

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

    private bool playerDataLoaded;

    void Awake()
    {
        inputManager = new InputManager();

        GameReferences.playerScript = this;
        GameReferences.player = this.gameObject;
    }

    void OnEnable()
    {
        inputManager.Player.Enable();
    }

    void OnDisable()
    {
        inputManager.Player.Disable();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y >= maxFallSpeed)
        {
            UnityEngine.Debug.Log("Player Fell Too Hard!");
            GameReferences.uIHandler.SendNotif("Player Fell! Yeouch!", 10f, Color.red);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        stamina = totalStamina;

        if (!playerDataLoaded)
        {
            Level l = GameReferences.levelGenerator.GetLevelInstance();
            Vector3 middleTopofWorld = new Vector3((l.Width / 2) * l.Scale, l.Height * l.Scale);
            UnityEngine.Debug.Log("Player Data Not Loaded! Setting Position to: " + middleTopofWorld);
            transform.position = middleTopofWorld;
            playerDataLoaded = true;
        }
    }

    private void Update()
    {
        GetInput(); // Get Data
    }

    void FixedUpdate()
    {
        MovePlayer(); // Do Physics
        MultiplyFall(); // Do Physics

        Animate(); // Update Visuals

        // Shitty Respawn to Middle of World If Below Certain Y Value or above certain X... yuck!

        if (transform.position.y < -10 || transform.position.x < 0 || transform.position.x > GameReferences.levelGenerator.GetLevelInstance().Width * GameReferences.levelGenerator.GetLevelInstance().Scale) 
        {
            Level l = GameReferences.levelGenerator.GetLevelInstance();
            Vector3 middleTopofWorld = new Vector3((l.Width / 2) * l.Scale, l.Height * l.Scale);
            UnityEngine.Debug.Log("Player Fell Below Level! Setting Position to: " + middleTopofWorld);
            transform.position = middleTopofWorld;
        }
    }

    public void GetInput()
    {
        input.x = inputManager.Player.Movement.ReadValue<float>();

        #region Run/Walk && Stamina

        if (Input.GetKey(KeyCode.LeftShift))
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

        if (Physics2D.OverlapCircle(grounded.position, groundedRadius, ground)) { if (inputManager.Player.Jump.triggered) { Jump(); }}
    }

    public void MovePlayer()
    {
        transform.position += input * speed * Time.fixedDeltaTime;
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, Vector3.zero.y);
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    public void MultiplyFall()
    {
        if (rb.velocity.y < minVertSpeed) 
        { 
            rb. velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; 
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
        float playerSpeed = GetPlayerSpeed();
        anim.SetFloat("playerSpeed", playerSpeed);
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
        float playerStamina = (stamina / totalStamina);
        return playerStamina;
    }

    #endregion

    #region Player Data Save/Load

    public void SaveAllPlayerData(string saveName)
    {
        PlayerSaveData playerDataToSave = new PlayerSaveData();

        // Current World Level Player is In
        playerDataToSave.levelIndex = level;

        // Player Pos
        playerDataToSave.x = Mathf.Round(transform.position.x * 100) / 100;
        playerDataToSave.y = Mathf.Round(Mathf.Ceil(transform.position.y) * 100) / 100;

        // Player Inventory
        GetComponent<Inventory>().SetInventoryToSave(saveName);
        playerDataToSave.playerInv = GetComponent<Inventory>().slotsToSave;

        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.SetPlayerDataSaveData(saveName, playerDataToSave);
    }

    public void LoadAllPlayerData(string saveName)
    {
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.LoadAllDataFromDisk(saveName);

        level = saveManager.loadedData.playerData.levelIndex;

        Vector3 loadedPos = new Vector3(saveManager.loadedData.playerData.x, saveManager.loadedData.playerData.y);
        transform.position = loadedPos;

        GetComponent<Inventory>().LoadInventory(saveName);

        playerDataLoaded = true;
    }

    #endregion
}
