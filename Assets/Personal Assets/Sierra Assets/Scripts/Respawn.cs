using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    public Vector3 startPosition;  // The start position to respawn the player
    public float fallThreshold = -10f;  // The Y-coordinate where the player falls below to trigger respawn
    private Rigidbody playerRigidbody;  // Reference to the player's Rigidbody component

    void Start()
    {
        // Get the Rigidbody component
        playerRigidbody = GetComponent<Rigidbody>();

        // Check if the Rigidbody is missing
        if (playerRigidbody == null)
        {
            // Log an error and add a Rigidbody to the player
            Debug.LogError("No Rigidbody found on the player! Adding one.");
            playerRigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }



    void Update()
    {
        // Check if the player's position falls below the threshold (for example, below Y = -10)
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    // Respawn the player at the starting position
    void Respawn()
    {
        // Reset the player's position to the start position
        transform.position = startPosition;

        // Optional: Reset velocity if you want to stop any falling physics
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;  // Stops any current motion
        }
    }
}
