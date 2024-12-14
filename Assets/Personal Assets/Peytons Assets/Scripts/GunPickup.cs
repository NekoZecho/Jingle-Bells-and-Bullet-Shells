using UnityEngine;

public class ItemDestroy : MonoBehaviour
{
    public GameObject itemToSpawn;  // Reference to the item prefab to spawn (set this in the inspector)
    private bool isPlayerInRange = false;
    private Transform playerTransform;  // To reference the player's transform

    // Optionally, you can define a spawn offset relative to the item, if needed
    public Vector3 spawnOffset = new Vector3(0, 1, 0); // Default offset, change in the Inspector if needed

    // This method is called when another collider enters the trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player (you can adjust this condition)
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;  // Get the player's transform
        }
    }

    // This method is called when another collider exits the trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;  // Clear player transform when they exit
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // If the player is within range and presses the "E" key, spawn the item and destroy this object
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SpawnItem();
            Destroy(gameObject);
        }
    }

    // Spawns the item at the spawn location with an optional offset and parents it to the player
    private void SpawnItem()
    {
        if (itemToSpawn != null && playerTransform != null)
        {
            // Instantiate the item at the player position plus the offset
            Vector3 spawnPosition = playerTransform.position + spawnOffset;

            // Spawn the item at the determined position
            GameObject spawnedItem = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
            spawnedItem.transform.SetParent(playerTransform);  // Parent it to the player
        }
    }
}
