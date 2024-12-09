using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    public Vector3 startPosition;  // The start position to respawn the player
    public float fallThreshold = -10f;  // The Y-coordinate where the player falls below to trigger respawn

    void Start()
    {
        // Save the starting position (initial position of the player)
        startPosition = transform.position;
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
        transform.position = startPosition;
    }
}
