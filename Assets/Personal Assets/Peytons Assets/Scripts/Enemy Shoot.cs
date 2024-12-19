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
    public float shootingRange = 10f;

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

    // New variables
    public float detectionDelay = 1f; // Time to wait before shooting after detecting the player
    private float detectionTimer = 0f; // Timer to track detection time
    private bool playerDetected = false; // Whether the player is detected or not

    [Header("Layer Mask for Obstructions")]
    public LayerMask obstructionLayerMask;

    void Start()
    {
        if (shootingParticles != null)
        {
            shootingParticles.Stop();
            var mainModule = shootingParticles.main;
            mainModule.prewarm = true;
        }

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        FindPlayerTarget();
    }

    void Update()
    {
        if (playerTarget != null)
        {
            AimAtPlayer();
            if (CanSeePlayer())
            {
                if (!playerDetected)
                {
                    // Player detected, start the detection timer
                    playerDetected = true;
                    detectionTimer = Time.time + detectionDelay;
                }

                if (playerDetected && Time.time >= detectionTimer)
                {
                    // Player has been detected and enough time has passed, check if we can shoot
                    if (CanShoot()) HandleShooting();
                }
            }
            else
            {
                // If the player is no longer visible, reset detection
                playerDetected = false;
            }
        }
        else
        {
            FindPlayerTarget();
        }
    }

    void FindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    void AimAtPlayer()
    {
        Vector2 directionToPlayer = (playerTarget.position - firingPoint.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        firingPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (playerTarget.position - firingPoint.position).normalized;
        float distanceToPlayer = Vector2.Distance(firingPoint.position, playerTarget.position);
        RaycastHit2D hit = Physics2D.Raycast(firingPoint.position, directionToPlayer, distanceToPlayer, obstructionLayerMask);
        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    bool CanShoot()
    {
        float distanceToPlayer = Vector2.Distance(firingPoint.position, playerTarget.position);
        return distanceToPlayer <= shootingRange && Time.time >= nextFireTime;
    }

    void HandleShooting()
    {
        Shoot();
        nextFireTime = Time.time + fireRate;
    }

    void Shoot()
    {
        if (projectilePrefab != null && firingPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
            float spread = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, spread) * firingPoint.right;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = direction * projectileSpeed;

            ShowMuzzleFlash();
            ChangeGunSprite(firingGunSprite);
            PlayGunfireSound();

            Destroy(projectile, 5f);
            Invoke(nameof(ResetGunSprite), gunFireSpriteDuration);
            EjectCasing();
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or Firing Point is not set!");
        }
    }

    void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firingPoint.position, firingPoint.rotation);
            muzzleFlash.transform.SetParent(firingPoint);
            Destroy(muzzleFlash, muzzleFlashDuration);
        }
        else
        {
            Debug.LogWarning("Muzzle Flash Prefab is not set!");
        }
    }

    void ChangeGunSprite(Sprite newSprite)
    {
        if (gunSpriteRenderer != null) gunSpriteRenderer.sprite = newSprite;
        else Debug.LogWarning("Gun SpriteRenderer is not set!");
    }

    void ResetGunSprite() => ChangeGunSprite(idleGunSprite);

    void PlayGunfireSound()
    {
        if (gunfireSound != null) audioSource.PlayOneShot(gunfireSound);
        else Debug.LogWarning("Gunfire sound is not assigned!");
    }

    void EjectCasing()
    {
        if (casingPrefab != null)
        {
            Vector3 casingPosition = firingPoint.position + firingPoint.TransformDirection(new Vector3(-0.2f, -0.1f, 0f));
            GameObject casing = Instantiate(casingPrefab, casingPosition, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

            Rigidbody2D casingRb = casing.GetComponent<Rigidbody2D>();
            if (casingRb != null)
            {
                float spreadAngle = Random.Range(-15f, 15f);
                Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * (Vector2.left + Vector2.down).normalized;
                casingRb.AddForce(firingPoint.TransformDirection(spreadDirection * casingEjectForce), ForceMode2D.Impulse);
                casingRb.AddTorque(Random.Range(-10f, 10f));
                StartCoroutine(FreezeCasingMovement(casingRb, casingFreezeTime));
            }

            Destroy(casing, casingLifetime);
        }
        else
        {
            Debug.LogWarning("Casing Prefab is not set!");
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

    void OnDrawGizmos()
    {
        if (shootingRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firingPoint.position, shootingRange);
        }
    }
}
