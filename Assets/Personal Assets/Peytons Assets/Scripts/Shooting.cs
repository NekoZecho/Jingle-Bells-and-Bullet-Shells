using UnityEngine;

public class RapidFireShooter2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Prefab for the projectile
    public Transform firingPoint;       // Where the projectile spawns
    public float projectileSpeed = 10f; // Speed of the projectile
    public float fireRate = 0.2f;       // Time between each shot (in seconds)
    public float bulletSpreadAngle = 5f; // Maximum spread angle in degrees

    private float nextFireTime = 0f;    // Tracks when the next shot can be fired

    void Update()
    {
        // Check if the left mouse button is being held down
        if (Input.GetMouseButton(0)) // 0 is the left mouse button
        {
            if (Time.time >= nextFireTime) // Ensure we wait for the fire rate
            {
                Shoot();
                nextFireTime = Time.time + fireRate; // Set the next allowed fire time
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firingPoint != null)
        {
            // Create the projectile at the firing point's position and rotation
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);

            // Apply bullet spread
            float spread = Random.Range(-bulletSpreadAngle, bulletSpreadAngle); // Random angle within spread range
            Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right; // Adjust the direction

            // Add velocity to the projectile's Rigidbody2D
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed; // Apply the randomized direction
            }

            // Optional: Destroy the projectile after a few seconds to clean up
            Destroy(projectile, 5f);
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or Firing Point is not set!");
        }
    }
}
