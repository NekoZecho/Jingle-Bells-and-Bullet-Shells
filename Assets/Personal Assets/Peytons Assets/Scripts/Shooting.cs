using System.Collections;
using System.Collections.Generic; // For List
using UnityEngine;
using UnityEngine.UI; // For UI.Image

public class RapidFireShooter2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firingPoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.2f;
    public float bulletSpreadAngle = 5f;

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

    [Header("Casing Settings")]
    public GameObject casingPrefab;
    public float casingEjectForce = 5f;
    public float casingLifetime = 5f;
    public float casingFreezeTime = 2f;

    [Header("Reload Settings")]
    public Animator gunAnimator;
    public string reloadTriggerName = "Reload";
    public bool isReloading = false;
    public float reloadTime = 2f;

    private int currentBullets;
    public int maxBullets = 10;

    [Header("Bullet UI Settings")]
    public List<Image> bulletImages; // References to UI Images for bullets
    public Sprite bulletFullSprite;  // Sprite for a full bullet
    public Sprite bulletEmptySprite; // Sprite for an empty bullet

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D playerRb;

    private float nextFireTime = 0f;

    void Start()
    {
        currentBullets = maxBullets;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Initialize UI bullets as full
        UpdateBulletUI();

        if (gunAnimator != null)
            gunAnimator.SetBool("Reload", false);
    }

    void Update()
    {
        HandleReloadInput();
        HandleShootingInput();
        HandleMovement();
    }

    void HandleReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            StartCoroutine(ReloadAnimation());
    }

    void HandleShootingInput()
    {
        if (isReloading) return;

        if (currentBullets <= 0) // Check if bullets are empty
        {
            if (!isReloading) // Ensure we only trigger reload once
                StartCoroutine(ReloadAnimation());
            return; // Skip shooting if out of bullets
        }

        if (holdToFire && Input.GetMouseButton(0))
            HandleShooting();
        else if (!holdToFire && Input.GetMouseButtonDown(0))
            HandleShooting();
    }

    void HandleShooting()
    {
        if (currentBullets > 0 && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firingPoint != null)
        {
            // Bullet shooting logic
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
            float spread = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = direction * projectileSpeed;

            ShowMuzzleFlash();
            ChangeGunSprite(firingGunSprite);
            PlayGunfireSound();
            EjectCasing();
            Destroy(projectile, 5f);

            // Update bullet count and UI
            currentBullets--;
            UpdateBulletUI();
            Invoke(nameof(ResetGunSprite), gunFireSpriteDuration);
        }
    }

    void UpdateBulletUI()
    {
        // Ensure the bullet UI reflects current bullets
        for (int i = 0; i < bulletImages.Count; i++)
        {
            if (i < currentBullets)
                bulletImages[i].sprite = bulletFullSprite; // Full bullet sprite
            else
                bulletImages[i].sprite = bulletEmptySprite; // Empty bullet sprite
        }
    }

    IEnumerator ReloadAnimation()
    {
        isReloading = true;
        gunAnimator?.SetBool("Reload", true);
        yield return new WaitForSeconds(reloadTime);

        currentBullets = maxBullets;
        UpdateBulletUI(); // Reset bullet UI after reload
        isReloading = false;
        gunAnimator?.SetBool("Reload", false);
    }

    void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firingPoint.position, firingPoint.rotation, firingPoint);
            Destroy(muzzleFlash, muzzleFlashDuration);
        }
    }

    void PlayGunfireSound()
    {
        if (gunfireSound != null && audioSource != null)
            audioSource.PlayOneShot(gunfireSound);
    }

    void EjectCasing()
    {
        if (casingPrefab != null)
        {
            Vector3 casingPos = firingPoint.position + firingPoint.TransformDirection(new Vector3(-0.2f, -0.1f, 0f));
            GameObject casing = Instantiate(casingPrefab, casingPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

            Rigidbody2D casingRb = casing.GetComponent<Rigidbody2D>();
            if (casingRb != null)
            {
                Vector2 baseDirection = (Vector2.left + Vector2.down).normalized;
                casingRb.AddForce(firingPoint.TransformDirection(baseDirection * casingEjectForce), ForceMode2D.Impulse);
                casingRb.AddTorque(Random.Range(-10f, 10f));

                StartCoroutine(FreezeCasingMovement(casingRb, casingFreezeTime));
            }

            Destroy(casing, casingLifetime);
        }
    }

    void ChangeGunSprite(Sprite newSprite)
    {
        if (gunSpriteRenderer != null)
            gunSpriteRenderer.sprite = newSprite;
    }

    void ResetGunSprite()
    {
        ChangeGunSprite(idleGunSprite);
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

    void HandleMovement()
    {
        if (playerRb != null)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(moveX, moveY).normalized;

            playerRb.linearVelocity = movement * moveSpeed;
        }
    }
}
