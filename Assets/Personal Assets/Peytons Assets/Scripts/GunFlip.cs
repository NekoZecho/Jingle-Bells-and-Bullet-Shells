using UnityEngine;

public class FlipObject : MonoBehaviour
{
    // The object to be flipped
    public Transform objectToFlip;

    void Update()
    {
        // Get the position of the mouse in the world
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the object to the mouse
        Vector3 directionToMouse = mousePosition - objectToFlip.position;

        // Check if the mouse is to the right or left of the object
        if (directionToMouse.x > 0)
        {
            // Mouse is on the right, make sure the object is not flipped
            if (objectToFlip.localScale.y < 0)
            {
                objectToFlip.localScale = new Vector3(objectToFlip.localScale.x, -objectToFlip.localScale.y, objectToFlip.localScale.z);
            }
        }
        else
        {
            // Mouse is on the left, flip the object on the Y-axis
            if (objectToFlip.localScale.y > 0)
            {
                objectToFlip.localScale = new Vector3(objectToFlip.localScale.x, -objectToFlip.localScale.y, objectToFlip.localScale.z);
            }
        }
    }
}
