using UnityEngine;

public class DeleteAndSpawn : MonoBehaviour
{
    public GameObject objectToSpawn;   // The object to spawn at the position where this object will be deleted

    void Update()
    {
        // Check if the E key is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Get the position of the current object (the object holding the script)
            Vector3 deletePosition = transform.position;

            // Destroy the current object (this object)
            Destroy(gameObject);

            // Spawn the new object at the current position
            if (objectToSpawn != null)
            {
                Instantiate(objectToSpawn, deletePosition, Quaternion.identity);
            }
        }
    }
}
