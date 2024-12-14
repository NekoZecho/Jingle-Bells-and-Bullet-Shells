using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.UI; // Required for UI elements

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;        // The maximum health of the player
    public int currentHealth;          // The player's current health
    public int damageFromEnemy = 10;   // Damage taken when touching an enemy

    [Header("UI Settings")]
    public Image healthBarFill;        // Reference to the health bar's fill image

    [Header("Visual Effects")]
    public SpriteRenderer playerSprite; // The player's main sprite
    public SpriteRenderer gunSprite;    // The gun's sprite
    public Color damageColor = Color.red; // Color when damaged
    public float damageEffectDuration = 0.2f; // How long the red effect lasts

    [Header("Blood Effect Settings")]
    public GameObject[] bloodPrefabs;  // Array of blood prefabs
    public float bloodSpawnRadius = 1.0f; // Radius around the player for random blood spawn

    private bool isTakingDamage = false; // Tracks if the player is in contact with an enemy
    private Color originalPlayerColor;   // Original color of the player's sprite
    private Color originalGunColor;      // Original color of the gun's sprite

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Store the original colors of the sprites
        if (playerSprite != null) originalPlayerColor = playerSprite.color;
        if (gunSprite != null) originalGunColor = gunSprite.color;
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent health from going below 0 or above maxHealth

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        UpdateHealthBar();

        FlashRed();  // Trigger the red flash effect
        DropBlood(); // Spawn blood effect when damage is taken

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method called when health reaches 0
    void Die()
    {
        Debug.Log("Player has died!");
        RestartGame();
    }

    // Restart the current scene
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Start taking damage when touching an enemy
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isTakingDamage)
        {
            isTakingDamage = true;
            TakeDamage(damageFromEnemy); // Apply damage instantly
        }
    }

    // Stop taking damage when no longer touching an enemy
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isTakingDamage = false;
        }
    }

    // Update the health bar UI
    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth; // Normalize health value between 0 and 1
        }
    }

    // Flash the player and gun sprites red
    void FlashRed()
    {
        if (playerSprite != null && gunSprite != null)
        {
            // Set the sprites to the damage color
            playerSprite.color = damageColor;
            gunSprite.color = damageColor;

            // Reset the sprites to their original colors after a delay
            Invoke(nameof(ResetSpriteColors), damageEffectDuration);
        }
    }

    // Reset the player and gun sprites to their original colors
    void ResetSpriteColors()
    {
        if (playerSprite != null) playerSprite.color = originalPlayerColor;
        if (gunSprite != null) gunSprite.color = originalGunColor;
    }

    // Spawn a random blood effect at a random position within a radius
    void DropBlood()
    {
        if (bloodPrefabs == null || bloodPrefabs.Length == 0)
        {
            Debug.LogWarning("No blood prefabs assigned! Please assign blood prefabs in the Inspector.");
            return;
        }

        // Generate a random position within a circle around the player
        Vector2 randomOffset = Random.insideUnitCircle * bloodSpawnRadius;
        Vector3 bloodSpawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

        // Choose a random blood prefab
        int randomIndex = Random.Range(0, bloodPrefabs.Length);

        // Spawn the blood prefab at the calculated position
        GameObject spawnedBlood = Instantiate(bloodPrefabs[randomIndex], bloodSpawnPosition, Quaternion.identity);

        // Optional: Add random rotation and scale for variety
        spawnedBlood.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f)); // Random Z rotation
        spawnedBlood.transform.localScale *= Random.Range(0.8f, 1.2f); // Random scaling
    }
}
