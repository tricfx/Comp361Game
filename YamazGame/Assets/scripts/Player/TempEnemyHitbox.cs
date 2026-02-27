
using UnityEngine;

public class TempEnemyHitbox : MonoBehaviour
{
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float damageCooldown = 1f;

    private float lastDamageTime = -999f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Damage player on contact
        if (other.CompareTag("PlayerHurtbox") && Time.time - lastDamageTime > damageCooldown)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                lastDamageTime = Time.time;
                Debug.Log("Enemy damaged player on contact!");
            }
        }
    }
}