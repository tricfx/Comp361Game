using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    [SerializeField] float _attackDamage = 1f;
    [SerializeField] float _knockbackForce = 15f;
    Collider2D hurtbox;

    void Start()
    {
        hurtbox = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitbox")) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player == null) return;

        Vector2 direction = (Vector2) (other.gameObject.transform.position - transform.parent.position).normalized;
        Vector2 knockback = direction * _knockbackForce;
        player.TakeDamage(_attackDamage, knockback);
    }
}
