using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 30f;
    private float currentHealth;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;
    private Color originalColor;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"Enemy spawned with {currentHealth} HP");

        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"TakeDamage called! isDead={isDead}, damage={damage}, currentHealth BEFORE={currentHealth}");

        if (isDead)
        {
            Debug.Log("Already dead, returning");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Health: {currentHealth}/{maxHealth}");

        // Flash red
        if (spriteRenderer)
        {
            spriteRenderer.color = hitColor;
            Invoke("ResetColor", 0.1f);
        }

        // SIMPLIFIED CHECK - force it to work
        Debug.Log($"Checking death: currentHealth={currentHealth}, is it <= 0? {currentHealth <= 0}");

        if (currentHealth <= 0)
        {
            Debug.Log("YES - Health <= 0, calling Die()");
            Die();
        }
        else
        {
            Debug.Log("NO - Health still above 0, not dying yet");
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer)
            spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        Debug.Log($"Die() called! isDead was {isDead}");

        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy died! Destroying in 0.5s");

        // Destroy after a delay
        Destroy(gameObject, 0.5f);
    }
}
