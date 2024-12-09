using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // Assign the objects you want to control in the inspector
    public GameObject[] inventoryObjects = new GameObject[4];

    void Start()
    {
        // Initially disable all inventory objects
        foreach (GameObject obj in inventoryObjects)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        // Check for key inputs and enable the corresponding object while disabling others
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchObject(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchObject(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchObject(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchObject(3);
        }
    }

    // Switch to the selected object
    void SwitchObject(int index)
    {
        // Disable all objects first
        foreach (GameObject obj in inventoryObjects)
        {
            obj.SetActive(false);
        }

        // Enable the selected object
        if (index >= 0 && index < inventoryObjects.Length)
        {
            inventoryObjects[index].SetActive(true);
        }
    }
}
