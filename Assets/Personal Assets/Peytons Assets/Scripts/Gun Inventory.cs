using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // Assign the objects you want to control in the inspector
    public GameObject[] inventoryObjects = new GameObject[9];

    void Start()
    {
        // Initially disable all inventory objects
        foreach (GameObject obj in inventoryObjects)
        {
            if (obj != null) // Check to avoid errors if some elements are not assigned
            {
                obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Check for key inputs and enable the corresponding object while disabling others
        for (int i = 0; i < inventoryObjects.Length; i++)
        {
            // Keys 1–9 correspond to KeyCode.Alpha1 to KeyCode.Alpha9
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchObject(i);
            }
        }
    }

    // Switch to the selected object
    void SwitchObject(int index)
    {
        // Disable all objects first
        foreach (GameObject obj in inventoryObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // Enable the selected object
        if (index >= 0 && index < inventoryObjects.Length && inventoryObjects[index] != null)
        {
            inventoryObjects[index].SetActive(true);
        }
    }
}
