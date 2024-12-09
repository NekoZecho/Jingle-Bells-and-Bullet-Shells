using UnityEngine;

public class VanishingPlatform : MonoBehaviour
{
    public float vanishDuration = 2f;  // Duration the platform stays invisible
    private Renderer platformRenderer;  // Renderer to hide/show the platform
    private Collider platformCollider;  // Collider to disable/enable platform interaction
    private bool isPlayerOnPlatform = false;  // Flag to track if player is on platform

    void Start()
    {
        // Ensure the platform has a renderer and collider
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();

        // Check if the platform has both components
        if (platformRenderer == null || platformCollider == null)
        {
            Debug.LogError("Platform is missing Renderer or Collider component.");
        }
    }

    void Update()
    {
        // This checks if the platform has been triggered to vanish and then reappear
        if (isPlayerOnPlatform)
        {
            VanishPlatform();
        }
    }

    // Trigger when the player lands on the platform
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
        }
    }

    // Trigger when the player leaves the platform
    void OnCollisionExit(Collision collision)
    {
        // Check if the object leaving is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
        }
    }

    // Vanish the platform
    void VanishPlatform()
    {
        // Hide the platform by disabling its renderer and collider
        platformRenderer.enabled = false;
        platformCollider.enabled = false;

        // After the vanish duration, make the platform reappear
        Invoke("ReappearPlatform", vanishDuration);
    }

    // Reappear the platform
    void ReappearPlatform()
    {
        // Enable the platform's renderer and collider
        platformRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
