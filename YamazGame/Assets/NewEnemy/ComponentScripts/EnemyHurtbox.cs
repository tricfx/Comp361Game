using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    [SerializeField] float attackDamage = 1f;
    [SerializeField] float knockbackForce = 15f;
    Collider2D hurtbox;

    void Start()
    {
        hurtbox = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitbox")) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null || player.feetCollider == null) return;

        NewEnemy enemy = transform.parent.parent.GetComponent<NewEnemy>();
        if (enemy == null || enemy.feetCollider == null) return;

        Vector2 enemyFeetPos = enemy.feetCollider.bounds.center;
        Vector2 playerFeetPos = player.feetCollider.bounds.center;

        Vector2 direction = (playerFeetPos - enemyFeetPos).normalized;
        Vector2 knockback = direction * knockbackForce;
        
        player.TakeDamage(attackDamage, knockback);
    }
}
