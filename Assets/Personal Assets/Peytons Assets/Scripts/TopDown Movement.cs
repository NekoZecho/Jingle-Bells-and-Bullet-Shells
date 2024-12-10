using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class TopDownMovementWithMouseFlip : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
    private Vector2 movement;    // Stores movement input
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    public Animator animator;

    // Audio components
    private AudioSource audioSource;
    public AudioClip moveSound; // Sound to play when moving

    void Start()
    {
        // Get the SpriteRenderer, Rigidbody2D, and AudioSource components
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Ensure the audio source does not loop
        audioSource.loop = false;
    }

    void Update()
    {
        // Get movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Set the Speed parameter for the animator based on movement magnitude
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Flip the sprite based on the mouse's position relative to the player
        if (mousePosition.x < transform.position.x)
            spriteRenderer.flipX = true; // Face left
        else
            spriteRenderer.flipX = false; // Face right

        // Play or stop the sound based on whether the player is moving
        if (movement.sqrMagnitude > 0 && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(moveSound); // Play the move sound
        }
        else if (movement.sqrMagnitude == 0 && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the sound if the player stops moving
        }
    }

    void FixedUpdate()
    {
        // Set the Rigidbody's velocity for movement
        rb.linearVelocity = movement.normalized * moveSpeed;
    }
}
