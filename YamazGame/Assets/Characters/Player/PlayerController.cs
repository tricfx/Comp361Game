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

    public float moveSpeed = 1000f;
    public float maxSpeed = 5f;
    public float idleFriction = 0.9f;

    public GameObject leftAttackHitbox;
    Vector2 moveInput = Vector2.zero;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    Collider2D leftAttackCollider;

    bool _canMove = true;
    bool _isMoving = false;

    // public ContactFilter2D movementFilter;
    // public float collisionOffset = 0.02f;
    // public SwordAttack swordAttack;
    // List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftAttackCollider = leftAttackHitbox.GetComponent<Collider2D>();
    }

    // Update is called once per frame
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
}
