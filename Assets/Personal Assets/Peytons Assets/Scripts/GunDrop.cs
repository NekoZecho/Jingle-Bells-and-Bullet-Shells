using UnityEngine;
using UnityEngine.UI;  // Add this to work with UI

public class DeleteAndSpawn : MonoBehaviour
{
    public GameObject objectToSpawn;   // The object to spawn at the position where this object will be deleted
    public string buttonTag = "DropButton";  // The tag of the button to find

    private Button dropButton;          // Reference to the UI Button
    private bool isObjectSpawned = false;  // Track if the object has already been spawned

    void Start()
    {
        // Find the button using the tag
        GameObject buttonObject = GameObject.FindGameObjectWithTag(buttonTag);

        // If the button is found, get the Button component
        if (buttonObject != null)
        {
            dropButton = buttonObject.GetComponent<Button>();
            dropButton.onClick.AddListener(DropObject);
        }
        else
        {
            Debug.LogWarning("Button with tag '" + buttonTag + "' not found.");
        }
    }

    void Update()
    {
        // Check if the E key is pressed
        if (Input.GetKeyDown(KeyCode.E) && !isObjectSpawned)
        {
            DropObject();
        }
    }

    // Method to delete the current object and spawn a new one
    public void DropObject()
    {
        if (!isObjectSpawned) // Ensure we only spawn once
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

            // Mark that the object has been spawned
            isObjectSpawned = true;
        }
    }
}
