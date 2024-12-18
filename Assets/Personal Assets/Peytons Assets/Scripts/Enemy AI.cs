using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TopDownEnemyAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public float speed = 3f, detectionRange = 5f, minimumDistance = 1f, radiusBuffer = 0.1f;
    public LayerMask obstacleLayer;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Blood Settings")]
    public List<GameObject> hitSpawnObjects;
    public GameObject deathSpawnObject;
    public float hitSpawnChance = 0.5f, hitSpawnRadius = 1f;

    [Header("Components")]
    public Transform player, weapon;
    private Rigidbody2D rb;
    private SpriteRenderer enemySpriteRenderer, weaponSpriteRenderer;
    private Sprite originalSprite;
    private Animator animator; // Reference to the Animator component

    [Header("Weapon Settings")]
    public List<Sprite> hitSprites;
    public float hitSpriteDuration = 0.2f, redDuration = 1f;

    [Header("Audio Settings")]
    public AudioClip hitSound; // Sound that plays when the enemy is hit
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        weaponSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Get Animator component
        originalSprite = enemySpriteRenderer.sprite;
        player = player ? player : GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;

        // Add or get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange && CanSeePlayer())
        {
            if (distanceToPlayer > minimumDistance - radiusBuffer && distanceToPlayer < minimumDistance + radiusBuffer)
                rb.linearVelocity = Vector2.zero;
            else if (distanceToPlayer > minimumDistance)
                ChasePlayer();
            else
                MoveAwayFromPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Update the Speed parameter in the Animator based on Rigidbody2D velocity
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    private bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, Vector2.Distance(transform.position, player.position), obstacleLayer);
        return hit.collider == null;
    }

    private void ChasePlayer()
    {
        MovePlayerDirection(player.position);
    }

    private void MoveAwayFromPlayer()
    {
        MovePlayerDirection(transform.position);
    }

    private void MovePlayerDirection(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;

        bool flipX = player.position.x < transform.position.x;
        enemySpriteRenderer.flipX = flipX;
        weapon.localScale = new Vector3(Mathf.Abs(weapon.localScale.x), flipX ? -Mathf.Abs(weapon.localScale.y) : Mathf.Abs(weapon.localScale.y), weapon.localScale.z);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hit sound
        if (hitSound && audioSource)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (hitSpawnObjects.Count > 0 && Random.value <= hitSpawnChance)
            Instantiate(hitSpawnObjects[Random.Range(0, hitSpawnObjects.Count)], (Vector2)transform.position + Random.insideUnitCircle * hitSpawnRadius, Quaternion.identity);
        if (currentHealth <= 0) Die();
        else if (hitSprites.Count > 0) StartCoroutine(TemporarySpriteChange());
        StartCoroutine(ChangeColorRed());
    }

    private void Die()
    {
        if (deathSpawnObject) Instantiate(deathSpawnObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator<WaitForSeconds> TemporarySpriteChange()
    {
        enemySpriteRenderer.sprite = hitSprites[Random.Range(0, hitSprites.Count)];
        yield return new WaitForSeconds(hitSpriteDuration);
        enemySpriteRenderer.sprite = originalSprite;
    }

    private IEnumerator ChangeColorRed()
    {
        enemySpriteRenderer.color = weaponSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(redDuration);
        enemySpriteRenderer.color = weaponSpriteRenderer.color = Color.white;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet) TakeDamage(bullet.damage);
            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, minimumDistance);
        if (player) Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, player.position);
    }
}
