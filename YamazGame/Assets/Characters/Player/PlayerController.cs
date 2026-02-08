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

    bool North {
        set {
            _north = value;
            animator.SetBool("north", _north);
        }
        get {
            return _north;
        }
    }

    bool West {
        set {
            _west = value;
            animator.SetBool("west", _west);
        }
        get {
            return _west;
        }
    }

    bool South {
        set {
            _south = value;
            animator.SetBool("south", _south);
        }
        get {
            return _south;
        }
    }

    bool East {
        set {
            _east = value;
            animator.SetBool("east", _east);
        }
        get {
            return _east;
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
    bool _north = false;
    bool _south = true;
    bool _east = false;
    bool _west = false;

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

            if (moveInput.x > 0) {
                East = true;
                West = false;   
            } else if (moveInput.x < 0) {
                East = false;
                West = true;
            } else {
                East = false;
                West = false;
            }

            if (moveInput.y > 0) {
                North = true;
                South = false;
            } else if (moveInput.y < 0) {
                North = false;
                South = true;
            } else {
                North = false;
                South = false;
            }

            IsMoving = true;
        } else {
            IsMoving = false;
        }
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
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
