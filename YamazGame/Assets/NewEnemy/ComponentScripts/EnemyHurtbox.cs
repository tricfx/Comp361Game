using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    public int AttackDamage
    {
        get
        {
            return _attackDamage;
        }
    }

    public int KnockbackForce
    {
        get
        {
            return _knockbackForce;
        }
    }

    [SerializeField] int _attackDamage = 2;
    [SerializeField] int _knockbackForce = 10;

    Collider2D hurtbox;

    void Start()
    {
        hurtbox = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        NewEnemy attacker = GetComponentInParent<NewEnemy>();
        if (attacker == null) return;

        // Damage player
        if (other.CompareTag("PlayerHitbox"))
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player == null) return;

            player.TakeDamage(_attackDamage);
            return;
        }

        // Damage enemy if on opposite team (for charm fights)
        NewEnemy targetEnemy = other.GetComponentInParent<NewEnemy>();

        if (targetEnemy != null && targetEnemy != attacker && targetEnemy.IsAlive && targetEnemy.CurrentTeam != attacker.CurrentTeam)
        {
            targetEnemy.TakeDamage(_attackDamage);
        }
    }
}
