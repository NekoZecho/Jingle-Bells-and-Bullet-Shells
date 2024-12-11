using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 20; // Default damage, can be changed in the Inspector

    // This method is called when the bullet collides with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the bullet hits something tagged "Enemy" or "Map"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the EnemyAI component and apply damage to the enemy
            TopDownEnemyAI enemy = collision.gameObject.GetComponent<TopDownEnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Apply bullet damage to the enemy
            }

            // Destroy the bullet after hitting an enemy
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Map"))
        {
            // Destroy the bullet when hitting the map
            Destroy(gameObject);
        }
    }
}
