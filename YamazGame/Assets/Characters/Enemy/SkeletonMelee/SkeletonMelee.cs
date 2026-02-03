using UnityEngine;

public class SkeletonMelee : Enemy
{

    bool CanAttack {
        set {
            _canAttack = value;
        }
        get {
            return _canAttack;
        }
    }

    bool Moving {
        set {
            _moving = value;
            animator.SetBool("moving", _moving);
        }
        get {
            return _moving;
        }
    }

    bool CanMove {
        set {
            _canMove = value;
        }
        get {
            return _canMove;
        }
    }
    
    public float moveSpeed = 500f;
    public float attackCooldown = 1f; 
    public GameObject attackHitbox;
    public DetectionZone detectionZone;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    DamageableCharacter damageableCharacter;
    Collider2D attackCollider;

    bool _canAttack = true;
    bool _moving = false;
    bool _canMove = true;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageableCharacter = GetComponent<DamageableCharacter>();
        attackCollider = attackHitbox.GetComponent<Collider2D>();
    }

    void FixedUpdate(){
        if (CanMove && damageableCharacter.Targetable && detectionZone.detectedObjects.Count > 0) {
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

    public override void Attack() {
        if (!CanAttack) return;

        CanAttack = false;
        animator.SetTrigger("attack");
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack() {
        CanAttack = true;
    }

    public void LockMovement() {
        CanMove = false;
    }

    public void UnlockMovement() {
        CanMove = true;
    }

}
