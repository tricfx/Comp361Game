using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        // Get PlayerHealth from parent
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit by enemy attack
        if (other.CompareTag("EnemyAttack") || other.CompareTag("Enemy"))
        {
            // Get damage from enemy
            int damage = 10; // Default

            // Try to get actual damage from enemy (Disabled for now, as it requires a specific EnemyAttack component)
            //var enemyAttack = other.GetComponent<EnemyAttack>();
            //if (enemyAttack != null)
                //damage = enemyAttack.damage;

            playerHealth?.TakeDamage(damage);
            Debug.Log($"Player hurtbox hit! Took {damage} damage");
        }
    }
}
