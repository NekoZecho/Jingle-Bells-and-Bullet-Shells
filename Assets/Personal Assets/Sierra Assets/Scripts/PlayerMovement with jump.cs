using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;  // Speed of movement
    [SerializeField] private float jumpForce = 7f;  // Force applied for jumping
    [SerializeField] private LayerMask groundLayer; // Layer used to detect ground
    [SerializeField] private Transform groundCheck; // Point used to check if player is grounded
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius for ground check

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MovePlayer();
        CheckGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal"); // Get left/right input
        float moveY = Input.GetAxis("Vertical");   // Get up/down input

        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;

        // Apply movement while keeping physics updates in sync
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGrounded()
    {
        // Check if the player is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the ground check in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
