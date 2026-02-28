using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    PlayerHealth playerHealth;

    private void Awake()
    {
        // Get PlayerHealth from parent
        playerHealth = GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning("Could not find PlayerHealth");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit by enemy attack
        if (other.CompareTag("EnemyHurtbox"))
        {
            // Get damage from enemy script
            EnemyHurtbox hurtbox = other.GetComponent<EnemyHurtbox>();
            if (hurtbox == null)
            {
                Debug.Log("Could not find EnemyHurtbox");
                return;
            }

            int damage = hurtbox.AttackDamage;

            // Try to get actual damage from enemy (Disabled for now, as it requires a specific EnemyAttack component)
            //var enemyAttack = other.GetComponent<EnemyAttack>();
            //if (enemyAttack != null)
                //damage = enemyAttack.damage;

            playerHealth?.TakeDamage(damage);
            Debug.Log($"Player hurtbox hit! Took {damage} damage");
        }
    }
}
