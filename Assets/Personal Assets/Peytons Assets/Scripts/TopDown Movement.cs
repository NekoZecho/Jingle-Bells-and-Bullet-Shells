using UnityEngine;
using UnityEngine.InputSystem;

public class MobileMovement : MonoBehaviour
{
    Vector2 moveVector;
    public float moveSpeed = 8f;

    // Reference to the center object (Transform)
    public Transform centerObject;

    // Maximum distance the player can move from the center object
    public float maxDistance = 5f;

    public void InputPlayer(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        // Ensure the center object is assigned
        if (centerObject == null)
        {
            Debug.LogError("Center Object not assigned.");
            return;
        }

        // Use Vector2 for 2D movement
        Vector2 movement = new Vector2(moveVector.x, moveVector.y);
        movement.Normalize();

        // Calculate the new position based on movement input
        Vector2 newPosition = (Vector2)transform.position + movement * moveSpeed * Time.deltaTime;

        // Calculate the distance from the center object
        float distanceFromCenter = Vector2.Distance(newPosition, centerObject.position);

        // If the player exceeds the max distance from the center, constrain the position
        if (distanceFromCenter > maxDistance)
        {
            // Calculate direction from the center to the player
            Vector2 directionToCenter = (newPosition - (Vector2)centerObject.position).normalized;

            // Set the new position to be at the maximum allowed distance from the center
            newPosition = (Vector2)centerObject.position + directionToCenter * maxDistance;
        }

        // Apply the constrained position to the player's transform
        transform.position = newPosition;
    }
}
