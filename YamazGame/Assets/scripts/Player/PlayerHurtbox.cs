using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    [SerializeField] public int attackDamage = 1;
    [SerializeField] private PlayerActions playerActions;
    public Collider2D attackCollider;

    [Header("Combo Multipliers")]
    public float attack3Multiplier = 1.5f;
    public float xtraMultiplier = 1.5f;
    public float xtraMultiplierChance = 0.08f; // 8%

    private void Awake()
    {
        if (attackCollider == null)
            Debug.LogWarning("Attack collider not set");

        if (!playerActions)
            playerActions = GetComponentInParent<PlayerActions>();
    }

    private int GetCurrentDamage()
    {
        int step = playerActions != null ? playerActions.ComboStep : 1;
        float multiplier = 1f;

        if (step == 3)
        {
            multiplier = attack3Multiplier;
            if (Random.value < xtraMultiplierChance)
            {
                multiplier *= xtraMultiplier;
                Debug.Log("CRIT! xtraMultiplier triggered on Attack 3!");
            }
        }

        return Mathf.Max(1, Mathf.RoundToInt(attackDamage * multiplier));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"PlayerHitbox collided with: {other.gameObject.name}, Tag: {other.tag}");

        if (playerActions == null || !playerActions.IsAttacking)
        {
            Debug.Log("Player is NOT attacking, ignoring collision");
            return;
        }

        if (!other.CompareTag("EnemyHitbox")) return;

        int damage = GetCurrentDamage();

        // Try regular enemy first
        NewEnemy enemy = other.GetComponentInParent<NewEnemy>();
        if (enemy != null)
        {
            Debug.Log($"Hit NewEnemy for {damage} damage (combo step {playerActions.ComboStep})");
            enemy.TakeDamage(damage);
            return;
        }

        // Try boss
        BossHitbox boss = other.GetComponentInParent<BossHitbox>();
        if (boss != null)
        {
            Debug.Log($"Hit Boss for {damage} damage (combo step {playerActions.ComboStep})");
            boss.TakeDamage(damage);
            return;
        }

        Debug.LogWarning($"Hit EnemyHitbox on {other.gameObject.name} but found no NewEnemy or BossHitbox on parent!");
    }
}
