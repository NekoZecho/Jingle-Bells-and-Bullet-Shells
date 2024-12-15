using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // Optional: Reference to the target, assigned dynamically by the script
    private Transform target;

    // Range of the line of sight check
    public float sightRange = 10f;

    // Optional: Layer mask for obstacles (to exclude certain layers from being detected)
    public LayerMask obstacleLayer;

    void Update()
    {
        // If the target is not assigned, find the GameObject with the "Player" tag
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // If the target is found and the player can be seen, make the object look at it
        if (target != null && CanSeePlayer())
        {
            // Get the target position in world space
            Vector3 targetPosition = target.position;

            // Calculate the direction from the object to the target
            Vector3 direction = targetPosition - transform.position;

            // Ensure the direction is in 2D by setting z to 0
            direction.z = 0;

            // Calculate the rotation angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation to the object
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Check if there are no obstacles between the object and the player
    bool CanSeePlayer()
    {
        if (target == null)
            return false;

        // Cast a ray from the object to the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, sightRange, obstacleLayer);

        // If the ray hits something, check if it's the player or an obstacle
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Player is in sight
                return true;
            }
            else
            {
                // An obstacle is blocking the view
                return false;
            }
        }

        // If no hit, player is in sight
        return true;
    }
}
