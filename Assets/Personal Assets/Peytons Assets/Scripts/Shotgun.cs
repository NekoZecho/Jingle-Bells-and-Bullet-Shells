using UnityEngine;

public class ShotgunShooter2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firingPoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f; // Increased fire rate for shotgun
    public float bulletSpreadAngle = 15f; // Spread for shotgun

    [Header("Fire Mode Settings")]
    public bool holdToFire = true; // Enable hold-to-fire functionality

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
    public AudioClip gunfireSound; // Sound effect for firing
    private AudioSource audioSource; // Audio source component

    private float nextFireTime = 0f;

    void Start()
    {
        if (shootingParticles != null)
        {
            shootingParticles.Stop();
            var mainModule = shootingParticles.main;
            mainModule.prewarm = true; // Enable prewarm
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If no AudioSource, add one
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (holdToFire && Input.GetMouseButton(0))
        {
            HandleShooting();
        }
        else if (!holdToFire && Input.GetMouseButtonDown(0))
        {
            HandleShooting();
        }
        else
        {
            StopShootingParticles();
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime) // Ensure shooting is at the correct rate
        {
            Shoot(); // Fire a shot
            nextFireTime = Time.time + fireRate; // Set the next fire time
        }

        PlayShootingParticles(); // Play particles while shooting
    }

    void Shoot()
    {
        if (projectilePrefab != null && firingPoint != null)
        {
            // Fire 3 bullets with spread
            for (int i = -1; i <= 1; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);

                // Calculate the spread based on the loop index (-1, 0, 1)
                float spread = i * bulletSpreadAngle;
                Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right;

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed; // Apply velocity
                }

                Destroy(projectile, 5f); // Destroy after 5 seconds

                // Play muzzle flash, sound, and gun sprite changes
                ShowMuzzleFlash();
                ChangeGunSprite(firingGunSprite);
                PlayGunfireSound();

                Invoke(nameof(ResetGunSprite), gunFireSpriteDuration); // Reset gun sprite after shooting
            }
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

    void PlayShootingParticles()
    {
        if (shootingParticles != null && !shootingParticles.isPlaying)
        {
            shootingParticles.Play(); // Start the particle system when shooting
        }
    }

    void StopShootingParticles()
    {
        if (shootingParticles != null && shootingParticles.isPlaying)
        {
            shootingParticles.Stop(); // Stop the particle system when the player stops shooting
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

    // Play the gunfire sound
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
}
