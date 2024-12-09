using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // How fast the player moves
    public float jumpForce = 10f; // How high the player jumps
    public LayerMask groundLayer; // To check if the player is on the ground

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Debugging: Check whether the player is grounded
        Debug.DrawRay(transform.position - new Vector3(0, 0.6f, 0), Vector2.down * 0.1f, Color.red); // Draw a ray for visualization

        // Check if the player is on the ground using a raycast slightly below the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.6f, 0), Vector2.down, 0.1f, groundLayer);
        isGrounded = hit.collider != null;

        // Debugging: Print if the player is grounded
        Debug.Log("Is Grounded: " + isGrounded);

        // Handle movement (horizontal input)
        float moveInput = Input.GetAxis("Horizontal");
        float targetSpeed = moveInput * moveSpeed;

        // Apply smooth movement, but only adjust the horizontal velocity (no vertical changes)
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);

        // Handle jumping (only if grounded)
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jumping!"); // Debug log for jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force
        }
    }
}
