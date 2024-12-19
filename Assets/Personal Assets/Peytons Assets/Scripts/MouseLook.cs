using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public string targetTag = "Aim"; // Tag of the object to look at
    private Transform target; // The object to look at

    void Start()
    {
        // Find the object with the "Aim" tag
        target = GameObject.FindGameObjectWithTag(targetTag)?.transform;

        // If no object with the specified tag is found, log a warning
        if (target == null)
        {
            Debug.LogWarning("No object found with the tag: " + targetTag);
        }
    }

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
