using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    Vector3 turning;
    public int turnSpeed;
    public int moveSpeed;

    void FixedUpdate()
    {
        // Checks if the player is moving w,a,s,d or arrow keys and sets their vectors
        velocity = new Vector3(Input.GetAxisRaw("Vertical"), 0, 0).normalized * moveSpeed;
        turning = new Vector3(0, Input.GetAxisRaw("Horizontal"), 0).normalized;

        // Moves and rotates their vectors based on the buttons player is holding down
        transform.Translate(velocity * Time.deltaTime);
        transform.Rotate(turning * Time.deltaTime * turnSpeed);
    }
}