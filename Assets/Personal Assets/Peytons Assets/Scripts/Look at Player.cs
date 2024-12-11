using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // Reference to the target object
    public Transform target;

    void Update()
    {
        if (target != null) // Ensure the target is assigned
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
