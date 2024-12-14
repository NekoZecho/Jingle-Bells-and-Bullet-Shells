using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // Optional: Reference to the target, assigned dynamically by the script
    private Transform target;

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

        // If the target is found, make the object look at it
        if (target != null)
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
}
