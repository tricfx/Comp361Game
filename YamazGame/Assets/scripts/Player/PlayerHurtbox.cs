using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    [SerializeField] public int attackDamage = 1;
    [SerializeField] private PlayerActions playerActions;
    public Collider2D attackCollider;

    [Header("Combo Multipliers")]
    [SerializeField] private float attack3Multiplier = 1.5f;
    [SerializeField] private float xtraMultiplier = 1.5f;
    [SerializeField] private float xtraMultiplierChance = 0.08f; // 8%

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

        if (playerActions != null && playerActions.IsAttacking)
        {
            Debug.Log($"Player IS attacking! Checking if enemy...");

            if (other.CompareTag("EnemyHitbox"))
            {
                Debug.Log($"Hit an enemy hitbox! Looking for EnemyHealth...");

                NewEnemy enemy = other.GetComponentInParent<NewEnemy>();
                if (enemy == null)
                {
                    Debug.LogWarning("Failed to find enemy component");
                    return;
                }

                int damage = GetCurrentDamage();
                Debug.Log($"Called TakeDamage on enemy for {damage} damage (combo step {playerActions.ComboStep})");
                enemy.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("Player is NOT attacking, ignoring collision");
        }
    }
}
