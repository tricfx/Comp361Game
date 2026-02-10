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
        GameObject player = other.transform.parent.gameObject;

        if (other.gameObject.CompareTag("Hitbox") && player.CompareTag("Player"))
        {
            Vector3 parentPosition = transform.parent.position;
            Vector2 direction = (Vector2) (other.gameObject.transform.position - parentPosition).normalized;
            Vector2 knockback = direction * _knockbackForce;
            player.GetComponent<PlayerController>().TakeDamage(_attackDamage, knockback);
        }
    }
}
