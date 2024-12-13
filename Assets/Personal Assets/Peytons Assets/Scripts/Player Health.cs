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

    private bool isTakingDamage = false; // Tracks if the player is in contact with an enemy

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent health from going below 0 or above maxHealth

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        UpdateHealthBar();

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
}
