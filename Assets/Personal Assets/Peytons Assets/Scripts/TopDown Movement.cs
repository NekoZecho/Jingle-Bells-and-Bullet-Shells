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

    // Particle systems for movement
    public ParticleSystem movementParticlesRight; // Particle effect when facing right
    public ParticleSystem movementParticlesLeft;  // Particle effect when facing left

    private bool isPlayingRightParticles = false; // To track right particle system state
    private bool isPlayingLeftParticles = false; // To track left particle system state

    void Start()
    {
        // Get the SpriteRenderer, Rigidbody2D, and AudioSource components
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Ensure the audio source does not loop
        audioSource.loop = false;

        // Ensure both particle systems are stopped at the start
        if (movementParticlesRight != null)
        {
            movementParticlesRight.Stop();
        }
        if (movementParticlesLeft != null)
        {
            movementParticlesLeft.Stop();
        }
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

        // Determine which direction the player is facing
        bool facingLeft = mousePosition.x < transform.position.x;
        spriteRenderer.flipX = facingLeft; // Flip sprite accordingly

        // Play the correct particle system based on direction and movement
        if (movement.sqrMagnitude > 0)
        {
            if (facingLeft)
            {
                if (!isPlayingLeftParticles && movementParticlesLeft != null)
                {
                    movementParticlesLeft.Play();
                    isPlayingLeftParticles = true;
                }
                if (isPlayingRightParticles && movementParticlesRight != null)
                {
                    movementParticlesRight.Stop();
                    isPlayingRightParticles = false;
                }
            }
            else
            {
                if (!isPlayingRightParticles && movementParticlesRight != null)
                {
                    movementParticlesRight.Play();
                    isPlayingRightParticles = true;
                }
                if (isPlayingLeftParticles && movementParticlesLeft != null)
                {
                    movementParticlesLeft.Stop();
                    isPlayingLeftParticles = false;
                }
            }
        }
        else
        {
            if (isPlayingLeftParticles && movementParticlesLeft != null)
            {
                movementParticlesLeft.Stop();
                isPlayingLeftParticles = false;
            }
            if (isPlayingRightParticles && movementParticlesRight != null)
            {
                movementParticlesRight.Stop();
                isPlayingRightParticles = false;
            }
        }

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
