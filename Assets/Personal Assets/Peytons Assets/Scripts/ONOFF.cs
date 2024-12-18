using UnityEngine;

public class ItemVisibilityTrigger2D : MonoBehaviour
{
    // The item you want to appear and disappear
    public GameObject item;

    private void Start()
    {
        // Make sure the item is invisible at the start
        if (item != null)
        {
            item.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other object is tagged with "Item"
        if (other.CompareTag("Item"))
        {
            // Make the item visible when in the trigger zone
            if (item != null)
            {
                item.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the other object is tagged with "Item"
        if (other.CompareTag("Item"))
        {
            // Make the item invisible when leaving the trigger zone
            if (item != null)
            {
                item.SetActive(false);
            }
        }
    }
}
