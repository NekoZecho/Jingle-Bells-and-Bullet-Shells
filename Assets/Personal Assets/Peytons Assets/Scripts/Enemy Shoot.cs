using UnityEngine;
using System.Collections;

public class EnemyShooter2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firingPoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.2f;
    public float bulletSpreadAngle = 5f;
    public float shootingRange = 10f; // Shooting range (radius)

    [Header("Muzzle Flash Settings")]
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.1f;

    [Header("Gun Sprite Settings")]
    public SpriteRenderer gunSpriteRenderer;
    public Sprite idleGunSprite;
    public Sprite firingGunSprite;
    public float gunFireSpriteDuration = 0.1f;

    [Header("Shooting Particle System")]
    public ParticleSystem shootingParticles;

    [Header("Audio Settings")]
    public AudioClip gunfireSound;
    private AudioSource audioSource;

    [Header("Casing Settings")]
    public GameObject casingPrefab;
    public float casingEjectForce = 5f;
    public float casingLifetime = 5f;
    public float casingFreezeTime = 2f;

    private float nextFireTime = 0f;
    private Transform playerTarget;

    [Header("Layer Mask for Obstructions")]
    public LayerMask obstructionLayerMask; // Specify layers to be considered as obstructions

    void Start()
    {
        if (shootingParticles != null)
        {
            shootingParticles.Stop();
            var mainModule = shootingParticles.main;
            mainModule.prewarm = true;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find player by tag at start
        FindPlayerTarget();
    }

    void Update()
    {
        if (playerTarget != null)
        {
            AimAtPlayer();

            // Check if player is within shooting range
            float distanceToPlayer = Vector2.Distance(firingPoint.position, playerTarget.position);
            if (distanceToPlayer <= shootingRange && CanSeePlayer())
            {
                HandleShooting();
            }
        }
        else
        {
            // Keep trying to find the player if it's not already set
            FindPlayerTarget();
        }
    }

    void FindPlayerTarget()
    {
        // Find the player object by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    void AimAtPlayer()
    {
        // Calculate the direction to the player
        Vector2 directionToPlayer = (playerTarget.position - firingPoint.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        firingPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    bool CanSeePlayer()
    {
        // Perform a raycast to check for obstructions
        Vector2 directionToPlayer = (playerTarget.position - firingPoint.position).normalized;
        float distanceToPlayer = Vector2.Distance(firingPoint.position, playerTarget.position);

        RaycastHit2D hit = Physics2D.Raycast(firingPoint.position, directionToPlayer, distanceToPlayer, obstructionLayerMask);

        // If the raycast hits something, it means the player is obstructed
        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firingPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);

            float spread = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }

            ShowMuzzleFlash();
            ChangeGunSprite(firingGunSprite);
            PlayGunfireSound();

            Destroy(projectile, 5f);
            Invoke(nameof(ResetGunSprite), gunFireSpriteDuration);

            // Eject casing for every shot
            EjectCasing();
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or Firing Point is not set!");
        }
    }

    void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && firingPoint != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firingPoint.position, firingPoint.rotation);
            muzzleFlash.transform.SetParent(firingPoint);
            Destroy(muzzleFlash, muzzleFlashDuration);
        }
        else
        {
            Debug.LogWarning("Muzzle Flash Prefab or Firing Point is not set!");
        }
    }

    void ChangeGunSprite(Sprite newSprite)
    {
        if (gunSpriteRenderer != null && newSprite != null)
        {
            gunSpriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("Gun SpriteRenderer or Sprite is not set!");
        }
    }

    void ResetGunSprite()
    {
        ChangeGunSprite(idleGunSprite);
    }

    void PlayGunfireSound()
    {
        if (gunfireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunfireSound);
        }
        else
        {
            Debug.LogWarning("Gunfire sound is not assigned or AudioSource is missing!");
        }
    }

    void EjectCasing()
    {
        if (casingPrefab != null && firingPoint != null)
        {
            // Offset position for the casing ejection
            Vector3 casingPosition = firingPoint.position + firingPoint.TransformDirection(new Vector3(-0.2f, -0.1f, 0f)); // Adjust as needed
            GameObject casing = Instantiate(casingPrefab, casingPosition, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

            // Apply a force to the casing to simulate ejection with spread
            Rigidbody2D casingRb = casing.GetComponent<Rigidbody2D>();
            if (casingRb != null)
            {
                // Adjust direction with random spread
                float spreadAngle = Random.Range(-15f, 15f); // Spread angle range in degrees
                Vector2 baseDirection = (Vector2.left + Vector2.down).normalized; // Base ejection direction
                Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * baseDirection;

                casingRb.AddForce(firingPoint.TransformDirection(spreadDirection * casingEjectForce), ForceMode2D.Impulse);
                casingRb.AddTorque(Random.Range(-10f, 10f)); // Add random spin

                // Freeze movement after a delay
                StartCoroutine(FreezeCasingMovement(casingRb, casingFreezeTime));
            }

            Destroy(casing, casingLifetime); // Destroy the casing after its lifetime
        }
        else
        {
            Debug.LogWarning("Casing Prefab or Firing Point is not set!");
        }
    }

    IEnumerator FreezeCasingMovement(Rigidbody2D casingRb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (casingRb != null)
        {
            casingRb.linearVelocity = Vector2.zero;
            casingRb.angularVelocity = 0f;
            casingRb.isKinematic = true;
        }
    }

    // Draw the shooting range radius in the Scene view
    void OnDrawGizmos()
    {
        if (shootingRange > 0)
        {
            Gizmos.color = Color.red; // Set the color of the radius
            Gizmos.DrawWireSphere(firingPoint.position, shootingRange); // Draw the wireframe circle
        }
    }
}
