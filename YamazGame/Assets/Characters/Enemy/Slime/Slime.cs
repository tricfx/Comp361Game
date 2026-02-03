using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Callbacks;
using UnityEngine;

public class Slime : MonoBehaviour {

    bool Moving {
        set {
            _moving = value;
            animator.SetBool("moving", _moving);
        }
        get {
            return _moving;
        }
    }

    public float damage = 1f;
    public float knockbackForce = 10f;
    public float moveSpeed = 350f;
    public DetectionZone detectionZone;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    DamageableCharacter damageableCharacter;

    bool _moving = false;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageableCharacter = GetComponent<DamageableCharacter>();
    }

    void FixedUpdate() {
        if (damageableCharacter.Targetable && detectionZone.detectedObjects.Count > 0) {
            Vector2 direction = (detectionZone.detectedObjects[0].transform.position - transform.position).normalized;
            rb.AddForce(direction * moveSpeed * Time.deltaTime);
            if (direction.x > 0) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
            Moving = true;
        } else {
            Moving = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        Collider2D collider = col.collider;
        IDamageable damageable = collider.GetComponent<IDamageable>();

        if (damageable != null && collider.gameObject.tag == "Player") {
            Vector2 direction = (collider.transform.position - transform.position).normalized;
            Vector2 knockback = direction * knockbackForce;
            damageable.OnHit(damage, knockback);
        }
    }
}
