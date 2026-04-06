using UnityEngine;

// Receives damage from player attacks (tag this GameObject "EnemyHitbox")
public class BossHitbox : MonoBehaviour
{
    private BossHealth bossHealth;

    private void Awake()
    {
        bossHealth = GetComponentInParent<BossHealth>();
        if (bossHealth == null)
            Debug.LogWarning("BossHitbox: Could not find BossHealth on parent!");
    }

    public void TakeDamage(int damage)
    {
        bossHealth?.TakeDamage(damage);
    }
}
