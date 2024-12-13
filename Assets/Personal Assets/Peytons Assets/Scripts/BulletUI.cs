using UnityEngine;
using UnityEngine.UI;

public class BulletUIScript : MonoBehaviour
{
    [Header("Bullet UI Settings")]
    public Image[] bulletIcons; // Array of Image components for bullet icons

    [Header("Shooting and Reloading Settings")]
    public int maxBullets = 10; // Maximum number of bullets
    private int currentBullets; // Current number of bullets

    void Start()
    {
        // Initialize bullet count and set up the UI
        currentBullets = maxBullets;
        InitializeBulletUI();
    }

    void InitializeBulletUI()
    {
        // Ensure there are enough bullet icons in the array
        if (bulletIcons.Length < maxBullets)
        {
            Debug.LogWarning("Not enough bullet icons assigned! You have " + bulletIcons.Length + " but maxBullets is set to " + maxBullets);
        }

        // Initialize the bullet icons visibility
        UpdateBulletUI();
    }

    public void UseBullet()
    {
        // If there are bullets remaining, disable the corresponding bullet icon
        if (currentBullets > 0)
        {
            currentBullets--;
            UpdateBulletUI();
        }
    }

    public void Reload()
    {
        // Reload the bullets (restore all icons to active)
        currentBullets = maxBullets;
        UpdateBulletUI();
    }

    private void UpdateBulletUI()
    {
        // Update the visibility of the bullet icons based on the current bullet count
        for (int i = 0; i < bulletIcons.Length; i++)
        {
            if (i < currentBullets)
            {
                bulletIcons[i].enabled = true; // Show the bullet icon
            }
            else
            {
                bulletIcons[i].enabled = false; // Hide the bullet icon
            }
        }
    }
}
