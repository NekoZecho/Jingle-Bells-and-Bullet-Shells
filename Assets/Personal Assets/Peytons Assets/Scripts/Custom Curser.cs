using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public float animationDuration = 1f; // Duration to play the animation
    private float timer = 0f; // Timer to track animation duration
    private bool isPlaying = false; // Tracks whether the animation is playing

    void Update()
    {
        // Convert screen position to world position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it's on the same plane
        transform.position = mousePosition;

        // Start playing animation if left mouse button is held
        if (Input.GetMouseButton(0))
        {
            if (!isPlaying)
            {
                animator.SetBool("Fire", true); // Trigger the animation
                isPlaying = true;
                timer = animationDuration; // Reset the timer
            }
        }

        // Update the timer if the animation is playing
        if (isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                animator.SetBool("Fire", false); // Stop the animation
                isPlaying = false;
            }
        }
    }
}
