using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public int maxHealth = 100;
    public int currentHealth;
    public int CurrentHealth => currentHealth; // For HUD update
    public int MaxHealth => maxHealth;         // For HUD update

    Rigidbody2D rb;

    [Header("References")]
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private PlayerActions actions;

    [Header("Death")]
    [SerializeField] private float deathPanelLength = 3f;
    [SerializeField] private GameObject gameOverAudioObject;
    private AudioSource[] gameOverSources;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private float deathPanelDelay = 0.5f;

    private bool isDead = false;
    public bool IsDead => isDead;
    public bool isInvincible = false;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.15f;

    [SerializeField] private AudioSource hitSound;

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (gameOverAudioObject != null)
            gameOverSources = gameOverAudioObject.GetComponents<AudioSource>();

        AudioListener.pause = false;

        isDead = false;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
        if (!actions) actions = GetComponent<PlayerActions>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Debug.Log("PlayerHealth Awake: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        hitSound.Play();
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");


        // CAMERA SHAKE
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.Shake();
        }

        if (currentHealth > 0 && currentHealth <= 30)
        {
            SoundManager.Instance?.TriggerLowHealthEffect(currentHealth);
        }

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            SoundManager.Instance?.StopLowHealthEffectImmediate();
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

        AudioListener.pause = true;

        if (gameOverSources != null && gameOverSources.Length > 0)
            StartCoroutine(PlayGameOverSequence());

        StartCoroutine(DeathSequence());
    }

    private void ReloadScene()
    {
        AudioListener.pause = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private System.Collections.IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
    }

    private System.Collections.IEnumerator PlayGameOverSequence()
    {
        foreach (AudioSource src in gameOverSources)
        {
            if (src == null || src.clip == null) continue;

            src.ignoreListenerPause = true;
            src.Stop();
            src.Play();

            yield return new WaitForSecondsRealtime(src.clip.length);
        }
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        if (deathPanel != null)
        {
            yield return new WaitForSecondsRealtime(deathPanelDelay);
            deathPanel.SetActive(true);
        }

        float remainingTime = Mathf.Max(0f, deathPanelLength - deathPanelDelay);
        yield return new WaitForSecondsRealtime(remainingTime);

        ReloadScene();
    }
}