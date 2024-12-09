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
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
    }

    void Update()
    {
        // Perform a raycast to check if the player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.6f, 0), Vector2.down, 0.1f, groundLayer);
        isGrounded = hit.collider != null;  // If the ray hits something, the player is grounded

        // Debugging: Print "Is Grounded: true" or "Is Grounded: false"
        Debug.Log("Is Grounded: " + isGrounded);

        // Handle movement (horizontal input)
        float moveInput = Input.GetAxis("Horizontal");
        float targetSpeed = moveInput * moveSpeed;

        // Apply smooth horizontal movement (but leave the vertical velocity unaffected)
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);

        // Handle jumping (only if the player is grounded)
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jumping!"); // Debug log for jump action
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force
        }
    }
}
