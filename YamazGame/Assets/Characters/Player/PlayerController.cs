using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    bool IsMoving {
        set {
            _isMoving = value;
            animator.SetBool("isMoving", _isMoving);
        }
        get {
            return _isMoving;
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

    [SerializeField] protected float moveSpeed = 1000f;
    [SerializeField] protected float maxSpeed = 5f;
    [SerializeField] protected float idleFriction = 0.9f;
    [SerializeField] protected GameObject leftAttackHitbox;

    public SpriteRenderer spriteRenderer { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public Collider2D feetCollider { get; private set; }

    protected Vector2 moveInput = Vector2.zero;
    protected bool _canMove = true;
    protected bool _isMoving = false;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        feetCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update() {
        
    }

    private void FixedUpdate() {
        if (CanMove && moveInput != Vector2.zero) {
            rb.AddForce(moveInput * moveSpeed * Time.fixedDeltaTime, ForceMode2D.Force);
            IsMoving = true;
        } else {
            IsMoving = false;
        }
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();

        if (CanMove && moveInput != Vector2.zero) {
            animator.SetFloat("xInput", moveInput.x);
            animator.SetFloat("yInput", moveInput.y);
            IsMoving = true;
        } else {
            IsMoving = false;
        }
    }

    void OnFire() {
        animator.SetTrigger("leftAttack");
    }

    public void LockMovement() {
        CanMove = false;
    }

    public void UnlockMovement() {
        CanMove = true;
    }

    public void TakeDamage(float damage, Vector2 knockback)
    {
        GetComponent<DamageableCharacter>().OnHit(damage, knockback);
    }
}
