using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public int maxHealth = 500;
    public int currentHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.15f;

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    [Header("Death")]
    [SerializeField] private float destroyDelay = 1f; // time to let death animation play

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Boss died!");
        // TODO: trigger death animation, rewards, cutscene etc.
        Destroy(gameObject, destroyDelay);
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
    }
}
