using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public Transform target; // The object to look at

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction from the object to the target
            Vector3 direction = target.position - transform.position;

            // Ensure the direction is in 2D by setting z to 0
            direction.z = 0;

            // Calculate the rotation angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation to the object
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
