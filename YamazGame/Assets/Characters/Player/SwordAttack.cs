using Unity.VisualScripting;
using UnityEngine;

public class SwordAttack : MonoBehaviour {

    public float swordDamage = 1f;
    public float knockbackForce = 15f;
    public Collider2D swordCollider;
    public Vector3 faceRight = new Vector3(0.064f, -0.05f, 0);
    public Vector3 faceLeft = new Vector3(-0.064f, -0.05f, 0);
    
    void Start() {
        if (swordCollider == null) {
            Debug.LogWarning("Sword Collider not set");
        }
    }

    void Update() {

    }

    void OnCollisionEnter2D(Collision2D collision) {
        IDamageable damageableObject = (IDamageable) collision.collider.GetComponent<IDamageable>();

        if (damageableObject != null) {
            Vector3 parentPosition = transform.parent.position;
            Vector2 direction = (Vector2) (collision.gameObject.transform.position - parentPosition).normalized;
            Vector2 knockback = direction * knockbackForce;
            damageableObject.OnHit(swordDamage, knockback);
        }
    }

    void IsFacingRight(bool isFacingRight) {
        gameObject.transform.localPosition = (isFacingRight) ? faceRight : faceLeft;
    }
}
