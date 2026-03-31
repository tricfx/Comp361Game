using UnityEngine;

// Deals damage to player (tag this GameObject "EnemyHurtbox")
public class BossHurtbox : MonoBehaviour
{
    [SerializeField] private int attackDamage = 20;
    public int AttackDamage => attackDamage;

    private BossAttack bossAttack;

    private void Awake()
    {
        bossAttack = GetComponentInParent<BossAttack>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitbox")) return;
        if (bossAttack != null && !bossAttack.IsAttacking) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        player?.TakeDamage(attackDamage);
    }
}
