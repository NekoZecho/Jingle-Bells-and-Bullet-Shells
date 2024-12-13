using System.Collections;
using UnityEngine;

public class RocketLauncher2D : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject rocketPrefab;  // The rocket prefab
    public Transform firingPoint;    // Firing point for the rocket
    public float rocketSpeed = 10f;  // Speed of the rocket
    public float fireRate = 1f;      // Fire rate for the launcher (slower than normal gun)
    public float rocketSpreadAngle = 10f;  // Spread for the rocket's trajectory

    [Header("Fire Mode Settings")]
    public bool holdToFire = true;  // Whether to hold to fire or press once

    [Header("Muzzle Flash Settings")]
    public GameObject muzzleFlashPrefab;  // Muzzle flash effect
    public float muzzleFlashDuration = 0.1f;

    [Header("Gun Sprite Settings")]
    public SpriteRenderer gunSpriteRenderer;
    public Sprite idleGunSprite;
    public Sprite firingGunSprite;
    public float gunFireSpriteDuration = 0.1f;

    [Header("Shooting Particle System")]
    public ParticleSystem shootingParticles;  // Particles for shooting

    [Header("Audio Settings")]
    public AudioClip rocketFireSound;  // Sound when firing the rocket
    private AudioSource audioSource;   // Audio source component

    [Header("Reload Settings")]
    public Animator gunAnimator;      // Animator for reloading
    public string reloadTriggerName = "Reload";  // Reload trigger name
    public bool isReloading = false;  // Track reload state
    public float reloadTime = 2f;     // Time for reloading

    [Header("Shooting Animation Settings")]
    public string shootTriggerName = "Shoot";  // Shooting animation trigger

    private float nextFireTime = 0f;

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

        // Ensure reload animation doesn't play on startup
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("Reload", false); // Reset reload state
        }
    }

    void Update()
    {
        // Check for reload input (R key) and if not already reloading
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadAnimation());
        }

        // If not reloading, handle firing input
        if (!isReloading)
        {
            if (holdToFire && Input.GetMouseButton(0)) // If holding to fire
            {
                HandleShooting();
            }
            else if (!holdToFire && Input.GetMouseButtonDown(0)) // If pressing once to fire
            {
                HandleShooting();
            }
            else
            {
                StopShootingParticles();
            }
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Set time for the next shot
        }

        PlayShootingParticles();
    }

    void Shoot()
    {
        if (rocketPrefab != null && firingPoint != null)
        {
            // Spawn the rocket at the firing point
            GameObject rocket = Instantiate(rocketPrefab, firingPoint.position, firingPoint.rotation);

            // Apply random spread to the rocket's direction
            float spread = Random.Range(-rocketSpreadAngle, rocketSpreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right; // Get direction with spread

            // Set the rocket's velocity to move it forward
            Rigidbody2D rb = rocket.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * rocketSpeed;  // Set rocket velocity
            }

            ShowMuzzleFlash();
            ChangeGunSprite(firingGunSprite);
            PlayRocketFireSound();

            // Play shooting animation
            PlayShootingAnimation();

            Destroy(rocket, 5f);  // Destroy rocket after 5 seconds
            Invoke(nameof(ResetGunSprite), gunFireSpriteDuration);

            // Eject casing for every shot
            EjectCasing();
        }
        else
        {
            Debug.LogWarning("Rocket Prefab or Firing Point is not set!");
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
        else
        {
            Debug.LogWarning("Gun SpriteRenderer or Sprite is not set!");
        }
    }

    void ResetGunSprite()
    {
        ChangeGunSprite(idleGunSprite); // Reset to idle sprite
    }

    void PlayRocketFireSound()
    {
        if (rocketFireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(rocketFireSound); // Play rocket fire sound
        }
        else
        {
            Debug.LogWarning("Rocket Fire Sound is not assigned or AudioSource is missing!");
        }
    }

    void PlayShootingAnimation()
    {
        if (gunAnimator != null && !string.IsNullOrEmpty(shootTriggerName))
        {
            gunAnimator.SetTrigger(shootTriggerName); // Play shooting animation
        }
        else
        {
            Debug.LogWarning("Gun Animator or Shoot Trigger is not set!");
        }
    }

    void EjectCasing()
    {
        // No casing eject for rockets, can leave this empty
    }

    IEnumerator ReloadAnimation()
    {
        isReloading = true; // Disable shooting
        gunAnimator.SetBool("Reload", true); // Start reload animation

        yield return new WaitForSeconds(reloadTime); // Wait for reload duration

        isReloading = false; // Re-enable shooting
        gunAnimator.SetBool("Reload", false); // End reload animation

        // Spawn a new rocket bullet after reloading
        Shoot();
    }

    // Call this when switching to this weapon
    public void OnWeaponSwitch()
    {
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("Reload", false); // Reset reload state when switching
        }

        if (gunSpriteRenderer != null && idleGunSprite != null)
        {
            ChangeGunSprite(idleGunSprite); // Ensure the weapon is in an idle state
        }
    }
}
