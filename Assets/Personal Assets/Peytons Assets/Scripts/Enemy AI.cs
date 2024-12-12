using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TopDownEnemyAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public float speed = 3f;
    public float detectionRange = 5f;
    public float minimumDistance = 1f; // Minimum distance from the player
    public float radiusBuffer = 0.1f; // Small buffer for the radius line

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Blood Settings")]
    public List<GameObject> hitSpawnObjects;
    public GameObject deathSpawnObject;
    public float hitSpawnChance = 0.5f; // Chance to spawn a hit object
    public float hitSpawnRadius = 1f;  // Radius for hit object spawn

    [Header("Components")]
    public Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer enemySpriteRenderer;  // Enemy's SpriteRenderer
    private SpriteRenderer weaponSpriteRenderer;  // Weapon's SpriteRenderer

    [Header("Weapon Settings")]
    public Transform weapon; // Weapon transform to flip with the player

    public List<Sprite> hitSprites;  // List of hit sprites
    public float hitSpriteDuration = 0.2f;
    private Sprite originalSprite;

    [Header("Enemy Hit Color Change")]
    public float redDuration = 1f; // Duration for turning red when hit

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get the SpriteRenderer components for the enemy and weapon
        enemySpriteRenderer = GetComponent<SpriteRenderer>();  // Assuming the enemy itself has a SpriteRenderer
        weaponSpriteRenderer = weapon.GetComponent<SpriteRenderer>();  // Weapon's SpriteRenderer

        originalSprite = enemySpriteRenderer.sprite;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Check if the player is on the border of the radius
            if (distanceToPlayer > minimumDistance - radiusBuffer && distanceToPlayer < minimumDistance + radiusBuffer)
            {
                // Stop the enemy from moving if the player is on the radius line
                rb.linearVelocity = Vector2.zero;
            }
            else if (distanceToPlayer > minimumDistance)
            {
                // Move towards the player if they are outside of minimum distance
                ChasePlayer();
            }
            else
            {
                // Move away from the player if they are within the minimum distance
                MoveAwayFromPlayer();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;  // Stop if the player is out of detection range
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Flip the enemy sprite based on the player's position
        bool flipX = player.position.x < transform.position.x;
        enemySpriteRenderer.flipX = flipX;

        // Flip the weapon's Y-axis based on the player's position
        if (weapon)
        {
            Vector3 weaponScale = weapon.localScale;
            weaponScale.y = flipX ? -Mathf.Abs(weaponScale.y) : Mathf.Abs(weaponScale.y);  // Flip on Y-axis
            weapon.localScale = weaponScale;
        }
    }

    private void MoveAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        rb.linearVelocity = direction * speed;

        // Flip the enemy sprite based on the player's position
        bool flipX = player.position.x < transform.position.x;
        enemySpriteRenderer.flipX = flipX;

        // Flip the weapon's Y-axis based on the player's position
        if (weapon)
        {
            Vector3 weaponScale = weapon.localScale;
            weaponScale.y = flipX ? -Mathf.Abs(weaponScale.y) : Mathf.Abs(weaponScale.y);  // Flip on Y-axis
            weapon.localScale = weaponScale;
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Spawn a hit object based on chance
        if (hitSpawnObjects.Count > 0 && Random.value <= hitSpawnChance)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * hitSpawnRadius;
            Instantiate(hitSpawnObjects[Random.Range(0, hitSpawnObjects.Count)], (Vector2)transform.position + spawnOffset, Quaternion.identity);
        }

        if (currentHealth <= 0)
            Die();
        else if (hitSprites.Count > 0)
            StartCoroutine(TemporarySpriteChange());

        // Trigger the red color change effect
        StartCoroutine(ChangeColorRed());
    }

    private void Die()
    {
        if (deathSpawnObject)
            Instantiate(deathSpawnObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator<WaitForSeconds> TemporarySpriteChange()
    {
        // Choose a random hit sprite from the list
        enemySpriteRenderer.sprite = hitSprites[Random.Range(0, hitSprites.Count)];
        yield return new WaitForSeconds(hitSpriteDuration);
        enemySpriteRenderer.sprite = originalSprite;
    }

    private IEnumerator ChangeColorRed()
    {
        // Change the color of the enemy's SpriteRenderer and weapon's SpriteRenderer to red
        enemySpriteRenderer.color = Color.red;
        weaponSpriteRenderer.color = Color.red;

        // Wait for the duration before reverting to the original color
        yield return new WaitForSeconds(redDuration);

        // Revert the color back to normal
        enemySpriteRenderer.color = Color.white;
        weaponSpriteRenderer.color = Color.white;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet)
                TakeDamage(bullet.damage);
            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitSpawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minimumDistance);
    }
}
