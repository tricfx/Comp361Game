using Unity.VisualScripting;
using UnityEngine;

public class SwordAttack : MonoBehaviour {

    public float swordDamage = 1f;
    public float knockbackForce = 15f;
    public Collider2D leftAttackCollider;
    
    void Start() {
        if (leftAttackCollider == null) {
            Debug.LogWarning("Sword Collider not set");
        }
    }

    void Update() {

    }

    void OnTriggerEnter2D(Collider2D col) {
        IDamageable damageableObject = (IDamageable) col.GetComponent<IDamageable>();

        if (damageableObject != null) {
            Vector3 parentPosition = transform.parent.position;
            Vector2 direction = (Vector2) (col.gameObject.transform.position - parentPosition).normalized;
            Vector2 knockback = direction * knockbackForce;
            damageableObject.OnHit(swordDamage, knockback);
        }
    }
}
