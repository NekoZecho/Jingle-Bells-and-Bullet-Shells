using UnityEngine;

public class TopDownMovementWithMouseFlip : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
    private Vector2 movement;    // Stores movement input
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Flip the sprite based on the mouse's position relative to the player
        if (mousePosition.x < transform.position.x)
            spriteRenderer.flipX = true; // Face left
        else
            spriteRenderer.flipX = false; // Face right
    }

    void FixedUpdate()
    {
        // Move the player
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
    }
}
