using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    public Transform target; // The object to follow
    public float speed = 5f; // Speed of following
    public Vector3 offset = Vector3.zero; // Offset from the target

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned!");
            return;
        }

        // Calculate the desired position with offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move towards the target
        transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
    }
}
