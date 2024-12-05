using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Speed of player movement
    [SerializeField] private float jumpForce = 7f; // Jump force applied to the player

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer; // The layer used to detect the ground
    [SerializeField] private Transform groundCheck; // The point to check if the player is grounded
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius of the ground check area

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void Update()
    {
        // Handle player movement and jumping
        MovePlayer();
        CheckGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right Arrow keys)

        // Apply movement to the Rigidbody2D on the X-axis, preserving the Y velocity (gravity & jump)
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        // Apply a force to make the player jump
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGrounded()
    {
        // Check if the player is grounded using a circle cast at the player's feet
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // For visualizing the ground check in the Scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
