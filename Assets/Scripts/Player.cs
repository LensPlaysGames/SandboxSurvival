using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;

    [Range(5f, 25f)]
    public float runSpeed, walkSpeed;
    public float totalStamina;
    public Color darkened, darker;
    [Range(1f, 20f)]
    public float defaultJumpForce, lesserJumpForce;
    [Range(2f, 20f)]
    public float fallMultiplier;
    [Range(0f, 18f)]
    public float minVertSpeed;
    [Range(0.01f, 10f)]
    public float groundedRadius;
    public LayerMask ground;

    private Vector3 input;
    public float speed, jumpForce, stamina;

    public Transform grounded;
    private Vector2 groundedPoint;

    private bool playerDataLoaded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (!playerDataLoaded)
        {
            World world = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>().GetWorldInstance();
            Vector3 middleTopofWorld = new Vector3(world.Width / world.scale, world.Height * world.scale);
            UnityEngine.Debug.Log("Player Data Not Loaded! Setting Position to: " + middleTopofWorld);
            transform.position = middleTopofWorld;
            playerDataLoaded = true;
        }

        stamina = totalStamina;
    }

    void FixedUpdate()
    {
        GetInput(); // Get Data

        MovePlayer(); // Do Physics
        MultiplyFall(); // Do Physics

        Animate(); // Update Visuals
    }

    public void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");

        // Run/Walk && Stamina
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                // Player Sprinting
                stamina -= Time.deltaTime; speed = runSpeed;
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
            if (stamina < totalStamina) { stamina += Time.deltaTime; }
            else if (stamina >= totalStamina) { stamina = totalStamina; }
            speed = walkSpeed;
            jumpForce = defaultJumpForce;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        // Jump
        groundedPoint = grounded.position;
        if (Physics2D.OverlapCircle(groundedPoint, groundedRadius, ground)) { if (Input.GetAxisRaw("Jump") != 0) { Jump(); } }
    }

    public void MovePlayer()
    {
        transform.position += input * speed * Time.fixedDeltaTime;
    }

    public void Jump()
    {
        rb.velocity = Vector3.zero;
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

    public void SavePlayerData(string saveName)
    {
        PlayerSaveData playerData = new PlayerSaveData();

        playerData.x = transform.position.x;
        playerData.y = transform.position.y;

        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.SetPlayerDataSaveData(saveName, playerData);
    }

    public void LoadPlayerData(string saveName)
    {
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.LoadAllDataFromDisk(saveName);

        Vector3 loadedPos = new Vector3(saveManager.loadedData.playerData.x, saveManager.loadedData.playerData.y);

        transform.position = loadedPos;

        playerDataLoaded = true;
    }

    #endregion

    #region DEBUG

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundedPoint, groundedRadius);
    }

    #endregion
}
