using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;

    [Range(5f, 25f)]
    public float runSpeed, walkSpeed;
    public Color darkened;
    [Range(1f, 20f)]
    public float defaultJumpForce, lesserJumpForce; 
    [Range(0.01f, 10f)]
    public float groundedRadius;
    public LayerMask ground;

    private Vector3 input;
    public float speed, jumpForce;

    public Transform grounded;
    private Vector2 groundedPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");

        // Run/Walk
        if (Input.GetKey(KeyCode.LeftShift)) { speed = walkSpeed; jumpForce = defaultJumpForce; GetComponent<SpriteRenderer>().color = darkened; }
        else { speed = runSpeed; jumpForce = lesserJumpForce; GetComponent<SpriteRenderer>().color = Color.white; }

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

    void FixedUpdate()
    {
        GetInput();
        MovePlayer();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundedPoint, groundedRadius);
    }
}
