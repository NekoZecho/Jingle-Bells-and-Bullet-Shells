using UnityEngine;
using System.Collections.Generic;

public class TopDownEnemyAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public float speed = 3f;
    public float detectionRange = 5f;

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
    private SpriteRenderer spriteRenderer;

    [Header("Weapon Settings")]
    public Transform weapon; // Weapon transform to flip with the player

    public List<Sprite> hitSprites;  // List of hit sprites
    public float hitSpriteDuration = 0.2f;
    private Sprite originalSprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
            ChasePlayer();
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Flip the sprite and the weapon
        bool flipX = direction.x < 0;
        spriteRenderer.flipX = flipX;

        if (weapon)
        {
            Vector3 weaponScale = weapon.localScale;
            weaponScale.y = flipX ? -Mathf.Abs(weaponScale.y) : Mathf.Abs(weaponScale.y);
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
        spriteRenderer.sprite = hitSprites[Random.Range(0, hitSprites.Count)];
        yield return new WaitForSeconds(hitSpriteDuration);
        spriteRenderer.sprite = originalSprite;
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
    }
}