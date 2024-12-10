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

    [Header("Spawn Settings")]
    public List<GameObject> hitSpawnObjects;
    public GameObject deathSpawnObject;

    [Header("Components")]
    public Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public List<Sprite> hitSprites;  // List of hit sprites
    public float hitSpriteDuration = 0.2f;
    private Sprite originalSprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;
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
        spriteRenderer.flipX = direction.x < 0;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (hitSpawnObjects.Count > 0)
            Instantiate(hitSpawnObjects[Random.Range(0, hitSpawnObjects.Count)], transform.position, Quaternion.identity);

        if (currentHealth <= 0) Die();
        else if (hitSprites.Count > 0) StartCoroutine(TemporarySpriteChange());
    }

    private void Die()
    {
        if (deathSpawnObject) Instantiate(deathSpawnObject, transform.position, Quaternion.identity);
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
            if (bullet) TakeDamage(bullet.damage);
            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected() =>
        Gizmos.DrawWireSphere(transform.position, detectionRange);
}
