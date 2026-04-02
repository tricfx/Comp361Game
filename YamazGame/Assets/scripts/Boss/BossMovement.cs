using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private BossAnimatorController anim;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 35f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 lastFacingDir = Vector2.down;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDir;

    public bool CanMove { get; set; } = true;
    public bool CanDash { get; set; } = false;
    public bool isInVertex = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<BossAnimatorController>();
    }

    private void FixedUpdate()
    {
        if (isInVertex)
        {
            Stop();
            return;
        }

        if (!CanMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        if (isDashing)
        {
            rb.linearVelocity = dashDir * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
                isDashing = false;

            anim?.SetSpeed(1f);
            anim?.SetMove(dashDir);
            return;
        }

        rb.linearVelocity = moveDir * moveSpeed;

        if (moveDir.sqrMagnitude > 0.001f)
            lastFacingDir = moveDir;

        anim?.SetSpeed(moveDir.magnitude);
        anim?.SetMove(lastFacingDir);
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDir = dir;

        if (dir.sqrMagnitude > 0.001f)
        {
            lastFacingDir = dir;
            anim?.SetFacing(dir);
            anim?.SetMove(dir);
        }
    }

    public void Dash(Vector2 direction)
    {
        if (!CanDash || isDashing || dashCooldownTimer > 0f) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDir = direction.normalized;

        if (dashDir.sqrMagnitude > 0.001f)
        {
            lastFacingDir = dashDir;
            anim?.SetFacing(dashDir);
            anim?.SetMove(dashDir);
        }
    }

    public void Stop()
    {
        moveDir = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        anim?.SetSpeed(0f);
    }
}
