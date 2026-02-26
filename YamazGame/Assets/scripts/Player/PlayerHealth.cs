using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth; // For HUD update
    public int MaxHealth => maxHealth;         // For HUD update

    Rigidbody2D rb;

    [Header("References")]
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private PlayerActions actions;

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        isDead = false;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
        if (!actions) actions = GetComponent<PlayerActions>();
        Debug.Log("PlayerHealth Awake: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // CAMERA SHAKE
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.Shake();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        rb.linearVelocity = Vector2.zero; // Stop all movement immediately

        // Trigger death animation
        anim?.TriggerDeath();

        // Disable player controls
        if (controller) controller.enabled = false;
        if (actions) actions.enabled = false;

        Invoke("ReloadScene", 3f);
    }

    private void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }


    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }


}
