using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // Variable initialization
    Rigidbody2D rb;

    // Variables for movement factors
    public float speed;
    public float jumpForce;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // Variables to make jumps only on the ground
    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    // Variables for jump delay
    public float rememberGroundedFor;
    float lastTimeGrounded;

    // Multiple jumps variable
    public int defaultAdditionalJumps = 1;
    int additionalJumps;



    void Start()
    {
        // Sets rigidbody and additional jumps
        rb = GetComponent<Rigidbody2D>();
        additionalJumps = defaultAdditionalJumps;
    }

    void Update()
    {

        // Simplifies the update function
        Move();
        Jump();
        BetterJump();
        CheckIfGrounded();
    }

    // Checks if they are using 
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        float moveBy = x * speed;

        rb.velocity = new Vector2(moveBy, rb.velocity.y);
    }

    // Checks if the player wants to jump and if they are ready
    void Jump()
    {
        if ( (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            additionalJumps--;
        }
    }

    // Gives a better feeling jump/fall
    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Checks if the player is on the ground
    void CheckIfGrounded()
    {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        // If the ground checker is near the ground change the variable and allow more jumps
        if (colliders != null)
        {
            isGrounded = true;
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

}
