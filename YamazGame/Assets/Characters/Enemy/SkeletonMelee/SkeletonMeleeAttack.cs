using UnityEngine;

public class SkeletonMeleeAttack : MonoBehaviour
{
    
    public float attackDamage = 2f;
    public float knockbackForce = 15f;
    public Collider2D attackCollider;
    public Vector3 faceRight = new Vector3(0.16f, -0.117f, 0);
    public Vector3 faceLeft = new Vector3(-0.16f, -0.117f, 0);
    
    void Start() {
        if (attackCollider == null) {
            Debug.LogWarning("Sword Collider not set");
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        IDamageable damageableObject = (IDamageable) collision.collider.GetComponent<IDamageable>();

        if (damageableObject != null && collision.gameObject.CompareTag("Player")) {
            Vector3 parentPosition = transform.parent.position;
            Vector2 direction = (Vector2) (collision.gameObject.transform.position - parentPosition).normalized;
            Vector2 knockback = direction * knockbackForce;
            damageableObject.OnHit(attackDamage, knockback);
        }
    }

}
