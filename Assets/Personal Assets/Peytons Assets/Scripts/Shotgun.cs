using UnityEngine;
using UnityEngine.UI;

public class ShotgunShooter2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firingPoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    public float bulletSpreadAngle = 15f;

    [Header("Fire Mode Settings")]
    public bool holdToFire = true;

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

    [Header("Animator Settings")]
    public Animator gunAnimator;  // Reference to the Animator

    [Header("Bullet UI Settings")]
    public int maxBullets = 6; // Maximum bullets in the shotgun
    public int currentBullets;
    public Image[] bulletImages; // Array to manually assign bullet images in the Inspector
    public Sprite activeBulletSprite; // Sprite for a loaded bullet
    public Sprite emptyBulletSprite;  // Sprite for an empty bullet

    [Header("Reload Settings")]
    public float reloadTime = 2f;
    private bool isReloading = false;

    private float nextFireTime = 0f;

    void Start()
    {
        currentBullets = maxBullets;

        // Initialize shooting particles
        if (shootingParticles != null)
        {
            shootingParticles.Stop();
            var mainModule = shootingParticles.main;
            mainModule.prewarm = true;
        }

        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Warn if animator is missing
        if (gunAnimator == null)
        {
            Debug.LogWarning("Animator not assigned!");
        }

        UpdateBulletUI();
    }

    void Update()
    {
        if (isReloading)
        {
            return; // Skip shooting logic while reloading
        }

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
            gunAnimator.SetBool("IsReloading", false); // Reset reloading if not shooting
        }

        // Start reload when bullets are empty
        if (currentBullets <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime && currentBullets > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        PlayShootingParticles();
    }

    void Shoot()
    {
        if (projectilePrefab != null && firingPoint != null)
        {
            // Trigger the shooting animation
            gunAnimator.SetTrigger("Shoot");

            // Fire 3 bullets with spread
            for (int i = -1; i <= 1; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
                float spread = i * bulletSpreadAngle;
                Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right;

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }

                Destroy(projectile, 5f);

                ShowMuzzleFlash();
                ChangeGunSprite(firingGunSprite);
                PlayGunfireSound();

                Invoke(nameof(ResetGunSprite), gunFireSpriteDuration);
            }

            // Deduct a bullet and update the UI
            currentBullets--;
            UpdateBulletUI();
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
    }

    void PlayShootingParticles()
    {
        if (shootingParticles != null && !shootingParticles.isPlaying)
        {
            shootingParticles.Play();
        }
    }

    void StopShootingParticles()
    {
        if (shootingParticles != null && shootingParticles.isPlaying)
        {
            shootingParticles.Stop();
        }
    }

    void ChangeGunSprite(Sprite newSprite)
    {
        if (gunSpriteRenderer != null && newSprite != null)
        {
            gunSpriteRenderer.sprite = newSprite;
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
    }

    void UpdateBulletUI()
    {
        if (bulletImages != null)
        {
            for (int i = 0; i < bulletImages.Length; i++)
            {
                if (i < currentBullets)
                {
                    bulletImages[i].sprite = activeBulletSprite;
                }
                else
                {
                    bulletImages[i].sprite = emptyBulletSprite;
                }
            }
        }
    }

    // Modified Reload function
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reloading...");
        gunAnimator.SetBool("IsReloading", true); // Trigger reload animation

        yield return new WaitForSeconds(reloadTime);

        currentBullets = maxBullets;  // Reload the bullets
        UpdateBulletUI();

        isReloading = false;
        gunAnimator.SetBool("IsReloading", false); // Reset the reload animation
    }
}
