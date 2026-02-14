using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("References")]
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private PlayerActions actions;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
        if (!actions) actions = GetComponent<PlayerActions>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

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

        // Trigger death animation
        anim?.TriggerDeath();

        // Disable player controls
        if (controller) controller.enabled = false;
        if (actions) actions.enabled = false;

        float deathAnimationLength = 2f; // Adjust to match your death animation length
        Destroy(gameObject, deathAnimationLength);

        // Optional: Reload scene after 3 seconds
        // Invoke("ReloadScene", 3f);
    }

    private void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
