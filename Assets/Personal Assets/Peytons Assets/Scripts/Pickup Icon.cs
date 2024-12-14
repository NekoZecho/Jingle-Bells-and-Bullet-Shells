using UnityEngine;

public class ToggleObjectInRadius : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The object that should be turned on/off.")]
    public GameObject targetObject;

    [Tooltip("The radius within which the object should be active.")]
    public float activationRadius = 5f;

    [Tooltip("The layer to detect (e.g., Player layer). Use 'Everything' to detect all.")]
    public LayerMask detectionLayer;

    private void Awake()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not assigned! Disabling script.");
            enabled = false;
        }
    }

    private void Update()
    {
        Collider2D detected = Physics2D.OverlapCircle(transform.position, activationRadius, detectionLayer);

        if (detected != null)
        {
            // If a valid object is detected in the radius, turn the targetObject on
            targetObject.SetActive(true);
        }
        else
        {
            // If no valid object is detected, turn the targetObject off
            targetObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the activation radius in the editor for visualization
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
