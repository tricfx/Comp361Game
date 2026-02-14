using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accel = 20f;
    [SerializeField] private float decel = 150f; // higher = slower to stop

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.8f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 lastAimDir = Vector2.down;

    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing;

    public Vector2 MoveDir => lastAimDir;
    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!input) input = GetComponent<PlayerInputHandler>();
        if (!anim) anim = GetComponent<PlayerAnimatorController>();

        // Ensure idle never starts at (0,0) if your idle tree defaults to east at (0,0)
        anim?.SetFacing(lastAimDir);
        anim?.SetMove(lastAimDir);
    }

    private void Update()
    {
        Vector2 move = input.Move;

        // Update last direction for facing (SNAP to cardinal to avoid wrong flashes)
        if (move.sqrMagnitude > 0.001f)
        {
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
                lastAimDir = new Vector2(Mathf.Sign(move.x), 0f);
            else
                lastAimDir = new Vector2(0f, Mathf.Sign(move.y));
        }

        // Dash trigger
        if (input.DashPressed && dashCooldownTimer <= 0f && !isDashing)
            StartDash();

        // Timers
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                EndDash();
        }

        // Animator parameters

        if (anim)
        {
            // deadzone tiny noise
            if (Mathf.Abs(move.x) < 0.01f) move.x = 0f;
            if (Mathf.Abs(move.y) < 0.01f) move.y = 0f;

            Vector2 moveDir = move.sqrMagnitude > 0.0001f ? move.normalized : Vector2.zero;

            // SAFETY: When not moving, send lastAimDir to MoveX/MoveY instead of (0,0)
            Vector2 safeMove = (moveDir.sqrMagnitude > 0.01f) ? moveDir : lastAimDir;

            anim.SetMove(safeMove);          // Use safeMove instead of moveDir
            anim.SetSpeed(move.magnitude);
            anim.SetFacing(lastAimDir);
            anim.SetDashing(isDashing);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = lastAimDir * dashSpeed;
            return;
        }

        Vector2 move = input.Move;
        Vector2 moveDir = move.sqrMagnitude > 0.0001f ? move.normalized : Vector2.zero;

        Vector2 target = moveDir * moveSpeed;

        // Use different acceleration when stopping
        float currentAccel = (moveDir == Vector2.zero) ? decel : accel;
        velocity = Vector2.MoveTowards(velocity, target, currentAccel * Time.fixedDeltaTime);

        rb.linearVelocity = velocity;
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        anim?.TriggerDash();
    }

    private void EndDash()
    {
        isDashing = false;
    }
}


