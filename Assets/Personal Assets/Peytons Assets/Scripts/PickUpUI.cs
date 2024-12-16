using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemTag = "Item"; // Tag of the item to trigger the spawn
    public GameObject objectToSpawn; // The object to spawn when touching the item
    public float anchorOffsetX = 1.0f; // X offset for the object position
    public float anchorOffsetY = 1.0f; // Y offset for the object position

    private GameObject spawnedObject;

    // Trigger detection when the player enters the item trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is tagged with "Item"
        if (other.CompareTag(itemTag) && spawnedObject == null) // Only spawn if not already spawned
        {
            SpawnObject();
        }
    }

    // Trigger detection when the player exits the item trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        // If the player exits the trigger area and the object exists, destroy it
        if (other.CompareTag(itemTag) && spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
    }

    private void SpawnObject()
    {
        // Instantiate the object to spawn and position it relative to the player's position
        spawnedObject = Instantiate(objectToSpawn, transform.position + new Vector3(anchorOffsetX, anchorOffsetY, 0), Quaternion.identity);

        // Parent the spawned object to the player to make it follow
        spawnedObject.transform.SetParent(transform);
    }
}
