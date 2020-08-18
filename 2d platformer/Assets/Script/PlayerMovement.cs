using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // Variable Initialzation 
    Rigidbody2D rigidBody;

    // For basic movement
    public float speed;
    public float jumpForce;

    // For jumping/falling information
    public float fallMultiplier = 2.5f;

    // How fast you fall from a low jump
    public float lowJumpMultiplier = 2f;

    // For making sure only certain amount of jumps before touching ground again
    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float rememberGroundedFor;
    float lastTimeGrounded;

    // Max amount of jumps in the air
    public int maxAdditionalJumps = 1;
    int additionalJumps;


    // Sets the additionalJumps variable and gets the rigid body
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        additionalJumps = maxAdditionalJumps;
    }

    // Calls movement based methods
    void Update()
    {
        Move();
        Jump();
        BetterJump();
        CheckIfGrounded();
    }

    // Moves the player left or right depending on input, and makes allows the player to keep current vertical velocity
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        float moveBy = x * speed;

        rigidBody.velocity = new Vector2(moveBy, rigidBody.velocity.y);
    }

    // Launches the player into the air based on jump force, allows player to keep horizontal velocity, or if player has more air jumps
    void Jump()
    {

        // if jump key is pressed, and player is on ground for long enough, or has more air jumps to use
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
            additionalJumps--;
        }
    }

    // A method to make jumping feel better
    void BetterJump()
    {
        if (rigidBody.velocity.y < 0)
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidBody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Checks if the player is on the ground
    void CheckIfGrounded()
    {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (colliders != null)
        {
            isGrounded = true;
            additionalJumps = maxAdditionalJumps;
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
