using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private PlayerActions playerActions;

    private void Awake()
    {
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
            if (other.CompareTag("EnemyHurtbox") || other.CompareTag("Enemy"))
            {
                Debug.Log($"Hit an enemy hurtbox! Looking for EnemyHealth...");

                // Try to find EnemyHealth on the enemy (might be on parent)
                var enemyHealth = other.GetComponent<EnemyHealth>();
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
                }
            }
        }
        else
        {
            Debug.Log("Player is NOT attacking, ignoring collision");
        }
    }
}
