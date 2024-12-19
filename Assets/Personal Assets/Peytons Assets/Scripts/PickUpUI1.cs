using UnityEngine;
using UnityEngine.UI;

public class UIButtonTrigger : MonoBehaviour
{
    public KeyCode keyToTrigger = KeyCode.E; // Default key to trigger

    public void TriggerKey()
    {
        Debug.Log($"{keyToTrigger} key triggered via button or keyboard");
        // Add your custom logic here
    }

    void Update()
    {
        // Check if the key is pressed and invoke the action
        if (Input.GetKeyDown(keyToTrigger))
        {
            TriggerKey();
        }
    }
}
