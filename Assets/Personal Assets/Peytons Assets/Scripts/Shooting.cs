using UnityEngine;

public class RapidFireShooter2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firingPoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.2f;  // Keep the fire rate but it won't affect particles
    public float bulletSpreadAngle = 5f;

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
        if (Input.GetMouseButton(0)) // Check if left mouse button is held
        {
            if (Time.time >= nextFireTime) // Ensure shooting is at the correct rate
            {
                Shoot(); // Fire a shot
                nextFireTime = Time.time + fireRate; // Set the next fire time
            }

            // Play the particle system continuously while shooting
            PlayShootingParticles();
        }
        else
        {
            // Stop the particle system when the mouse button is released
            StopShootingParticles();
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

            // Play the gunfire sound every time a bullet is fired
            PlayGunfireSound();

            Destroy(projectile, 5f);
            Invoke(nameof(ResetGunSprite), gunFireSpriteDuration);
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
