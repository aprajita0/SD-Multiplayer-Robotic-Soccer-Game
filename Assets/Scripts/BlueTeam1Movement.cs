using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueTeam1Movement : MonoBehaviour
{
    public float moveSpeed = 35f; // Speed of movement
    public float gravity = -9.81f; // Gravity effect

    private Vector3 moveDirection;
    private float verticalVelocity = 0f; // Vertical velocity for gravity
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component for physics-based movement
        rb = GetComponent<Rigidbody>();
        
        // Make sure the Rigidbody is not set to Kinematic (we want it to react to physics)
        rb.useGravity = true;
        
        // Freeze rotation on X and Z axes to prevent tilting
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Check for horizontal (A/D or Left/Right keys) and vertical (W/S or Up/Down keys) input
        float moveZ = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveX = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Calculate the movement direction
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Apply gravity manually if Rigidbody gravity is disabled or unwanted
        if (!rb.useGravity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Move the capsule by directly modifying the Rigidbody's velocity while locking the rotation
        HandleMovement();
    }

    void HandleMovement()
    {
        // Set the Rigidbody's velocity based on the movement direction
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("SoccerBall")) // Ensure the ball is tagged as "Ball"
        {
            Rigidbody ballRigidbody = collision.collider.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                // Calculate the direction of the push (only horizontal, ignoring the y-axis)
                Vector3 pushDirection = collision.contacts[0].point - transform.position;
                pushDirection.y = 0; // Prevent the ball from being pushed upwards

                // Apply the force in the horizontal direction (like a soccer kick)
                ballRigidbody.AddForce(pushDirection.normalized * moveSpeed, ForceMode.Impulse); // Apply the force to the ball
            }
        }
    }
}
