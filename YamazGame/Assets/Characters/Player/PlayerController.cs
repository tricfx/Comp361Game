using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    bool IsMoving {
        set {
            isMoving = value;
            animator.SetBool("isMoving", isMoving);
        }
    }

    public float moveSpeed = 1000f;
    public float maxSpeed = 5f;

    // each frame of physics, what percentage of the speed should be shaved off the velocity out of 1 (100%)
    public float idleFriction = 0.9f;

    public GameObject swordHitbox;

    Vector2 moveInput = Vector2.zero;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    Collider2D swordCollider;

    bool canMove = true;
    bool isMoving = false;

    // public ContactFilter2D movementFilter;
    // public float collisionOffset = 0.02f;
    // public SwordAttack swordAttack;
    // List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        swordCollider = swordHitbox.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void FixedUpdate() {

        if (canMove && moveInput != Vector2.zero) {
            // move animation and velocity

            // accelerate player while run direction is inputted
            // cap movement speed at maxSpeed for any direction
            // rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity + (moveInput * moveSpeed * Time.deltaTime), maxSpeed);

            rb.AddForce(moveInput * moveSpeed * Time.fixedDeltaTime, ForceMode2D.Force);

            /* if (rb.linearVelocity.magnitude > maxSpeed) {
                float limitedSpeed = Mathf.Lerp(rb.linearVelocity.magnitude, maxSpeed, idleFriction);
                rb.linearVelocity = rb.linearVelocity.normalized * limitedSpeed;
            } */

            // looking left or right
            if (moveInput.x > 0) {
                spriteRenderer.flipX = false;
                gameObject.BroadcastMessage("IsFacingRight", true);
            } else if (moveInput.x < 0) {
                spriteRenderer.flipX = true;
                gameObject.BroadcastMessage("IsFacingRight", false);
            }

            IsMoving = true;

        } else {
            // no movement, interpolate velocity towards 0
            // rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, idleFriction);

            IsMoving = false;
        }
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    /* public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    } */
}
