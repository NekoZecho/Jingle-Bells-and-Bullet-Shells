using UnityEngine;

public class FlipObject : MonoBehaviour
{
    // The object to be flipped
    public Transform objectToFlip;

    // Sorting order and layer values
    public int sortingOrderWhenLeft = 5;
    public string sortingLayerWhenLeft = "Foreground";
    public int sortingOrderWhenRight = 0;
    public string sortingLayerWhenRight = "Default";

    private SpriteRenderer spriteRenderer;

    // Reference to the "Aim" tagged object
    public string aimTag = "Aim";

    void Start()
    {
        // Get the SpriteRenderer component from the object to flip
        spriteRenderer = objectToFlip.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the object to flip!");
        }
    }

    void Update()
    {
        // Find the object with the "Aim" tag
        GameObject aimObject = GameObject.FindWithTag(aimTag);
        if (aimObject != null)
        {
            // Calculate the direction from the object to the "Aim" object
            Vector3 directionToAim = aimObject.transform.position - objectToFlip.position;

            // Check if the "Aim" object is to the right or left of the object
            if (directionToAim.x > 0)
            {
                // "Aim" is on the right, make sure the object is not flipped
                if (objectToFlip.localScale.y < 0)
                {
                    objectToFlip.localScale = new Vector3(objectToFlip.localScale.x, -objectToFlip.localScale.y, objectToFlip.localScale.z);
                }

                // Change sorting order and layer for the right side
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = sortingOrderWhenRight;
                    spriteRenderer.sortingLayerName = sortingLayerWhenRight;
                }
            }
            else
            {
                // "Aim" is on the left, flip the object on the Y-axis
                if (objectToFlip.localScale.y > 0)
                {
                    objectToFlip.localScale = new Vector3(objectToFlip.localScale.x, -objectToFlip.localScale.y, objectToFlip.localScale.z);
                }

                // Change sorting order and layer for the left side
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = sortingOrderWhenLeft;
                    spriteRenderer.sortingLayerName = sortingLayerWhenLeft;
                }
            }
        }
    }
}
