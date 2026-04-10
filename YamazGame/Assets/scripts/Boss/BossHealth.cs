using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEditor.Callbacks;


public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public int maxHealth = 500;
    public int currentHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public LevelLoader levelLoader;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private BossRoomController roomController;
    [SerializeField] private BossAnimatorController bossAnim;
    [SerializeField] private float deathAnimDuration = 1.33f; // match your death clip length
    [SerializeField] private GameObject rosePrefab; // spawned at death position in Phase 2

    [Header("Death")]
    [SerializeField] private float destroyDelay = 1f; // time to let death animation play

    private bool isDead = false;
    private Rigidbody2D _rigidbody2D;
    public bool IsDead => isDead;
    public Vector2 DeathPosition { get; private set; } // tracks boss position on death
    private EnemyManager _enemyManager;


    private void Awake()
    {
        currentHealth = maxHealth;
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!bossAnim) bossAnim = GetComponent<BossAnimatorController>();
        _enemyManager = FindFirstObjectByType<EnemyManager>();
        if (_enemyManager) _enemyManager.RegisterEnemy();
        if (!roomController) roomController = FindFirstObjectByType<BossRoomController>();
        if (!levelLoader) levelLoader = FindFirstObjectByType<LevelLoader>();
        if (!_rigidbody2D) _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    [System.Obsolete]
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            _rigidbody2D.linearVelocity = Vector2.zero; // Stop movement immediately
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Store position for later use
        DeathPosition = transform.position;

        Debug.Log($"Boss died at {DeathPosition}!");

        // 1 — Stop all movement and physics immediately
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.bodyType = RigidbodyType2D.Static; // prevents any further physics movement

        // 2 — Disable AI, attack and movement scripts so boss can't act after death
        BossAI ai = GetComponent<BossAI>();
        if (ai) ai.enabled = false;

        BossAttack attack = GetComponent<BossAttack>();
        if (attack) attack.enabled = false;

        BossMovement movement = GetComponent<BossMovement>();
        if (movement) movement.enabled = false;

        // 3 — Disable all hurtboxes so boss can't deal damage after death
        foreach (var hurtbox in GetComponentsInChildren<BossHurtbox>())
            hurtbox.enabled = false;

        // 4 — Disable all hitboxes so boss can't receive damage after death
        foreach (var hitbox in GetComponentsInChildren<BossHitbox>())
            hitbox.enabled = false;

        // 5 — Trigger death animation
        bossAnim?.TriggerDeath();

        // Wait for death animation, then do everything else
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Wait for the full death animation to finish
        yield return new WaitForSeconds(deathAnimDuration);

        // Spawn rose at death position in Phase 2 — must be BEFORE Destroy
        Debug.Log($"[Rose] Scene: '{SceneManager.GetActiveScene().name}' | rosePrefab: {(rosePrefab != null ? rosePrefab.name : "NULL")} | DeathPosition: {DeathPosition}");
        if (rosePrefab != null && SceneManager.GetActiveScene().name == "Boss-Phase-2")
        {
            Debug.Log("[Rose] Spawning rose at " + DeathPosition);
            Instantiate(rosePrefab, DeathPosition, Quaternion.identity);
        }
        else
        {
            if (rosePrefab == null) Debug.LogWarning("[Rose] rosePrefab is NULL — assign it in Inspector");
            if (SceneManager.GetActiveScene().name != "Boss-Phase-2") Debug.LogWarning($"[Rose] Wrong scene: '{SceneManager.GetActiveScene().name}' expected 'Boss-Phase-2'");
        }

        // Destroy boss object immediately after animation
        Destroy(gameObject);

        // Post-death world effects
        roomController?.UnlockArena();

        GameObject obj = GameObject.Find("OverworldMusic");
        if (obj != null)
        {
            AudioFader fader = obj.GetComponent<AudioFader>();
            if (fader != null) fader.FadeOut(2f);
        }

        if (_enemyManager) _enemyManager.UnregisterEnemy();

        TimeManager timeManager = FindFirstObjectByType<TimeManager>();
        if (timeManager != null) timeManager.StopTimerAndSubmit();

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex == 9)
            levelLoader.LoadLevel(currentIndex + 1);
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
    }


}
