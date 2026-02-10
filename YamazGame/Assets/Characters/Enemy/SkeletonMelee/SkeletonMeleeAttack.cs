using UnityEngine;

public class SkeletonMeleeAttack : MonoBehaviour
{
    
    public float attackDamage = 2f;
    public float knockbackForce = 15f;
    public Collider2D attackCollider;
    
    void Start() {
        if (attackCollider == null) {
            Debug.LogWarning("Sword Collider not set");
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        IDamageable damageableObject = (IDamageable) col.GetComponent<IDamageable>();

        if (damageableObject != null && col.gameObject.CompareTag("Player")) {
            Vector3 parentPosition = transform.parent.position;
            Vector2 direction = (Vector2) (col.gameObject.transform.position - parentPosition).normalized;
            Vector2 knockback = direction * knockbackForce;
            damageableObject.OnHit(attackDamage, knockback);
        }
    }

}
