using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    [SerializeField] public float attackDamage = 1f;
    [SerializeField] private PlayerActions playerActions;
    public Collider2D attackCollider;

    private void Awake()
    {
        if (attackCollider == null)
        {
            Debug.LogWarning("Attack collider not set");
        }

        if (!playerActions)
            playerActions = GetComponentInParent<PlayerActions>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"PlayerHitbox collided with: {other.gameObject.name}, Tag: {other.tag}");

        // Only damage enemies if player is attacking
        if (playerActions != null && playerActions.IsAttacking)
        {
            Debug.Log($"Player IS attacking! Checking if enemy...");

            // Check if we hit an enemy's hurtbox
            if (other.CompareTag("EnemyHitbox"))
            {
                Debug.Log($"Hit an enemy hitbox! Looking for EnemyHealth...");

                NewEnemy enemy = other.GetComponentInParent<NewEnemy>();
                if (enemy == null)
                {
                    Debug.LogWarning("Failed to find enemy component");
                }

                Debug.Log($"Called TakeDamage on enemy for {attackDamage} damage");
                enemy.TakeDamage(attackDamage);

                // Try to find EnemyHealth on the enemy (might be on parent)
                /* var enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth == null)
                    enemyHealth = other.GetComponentInParent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    Debug.Log($"Called TakeDamage on enemy for {attackDamage} damage!");
                }
                else
                {
                    Debug.LogError("Found enemy hurtbox but NO EnemyHealth component!");
                } */
            }
        }
        else
        {
            Debug.Log("Player is NOT attacking, ignoring collision");
        }
    }
}
