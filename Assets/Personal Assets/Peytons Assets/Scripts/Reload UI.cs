using UnityEngine;

public class ActivateOnKeyPress : MonoBehaviour
{
    public GameObject targetObject;  // The object to be activated
    public float activationTime = 5f; // Time in seconds before the object is disabled

    private void Update()
    {
        // Check if the R key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Activate the object
            if (targetObject != null)
            {
                targetObject.SetActive(true);
                // Start the coroutine to deactivate the object after a delay
                StartCoroutine(DeactivateObjectAfterTime());
            }
        }
    }

    private System.Collections.IEnumerator DeactivateObjectAfterTime()
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(activationTime);

        // Deactivate the object
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
