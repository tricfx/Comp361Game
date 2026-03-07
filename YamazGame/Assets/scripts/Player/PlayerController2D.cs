using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerActions actions;

    [Header("Move")]
    public float moveSpeed = 10f;
    public float accel = 60f;
    public float decel = 120f; // higher = slower to stop

    [Header("Dash")]
    public float dashSpeed = 20f;       // How fast the dash moves
    public float dashDistance = 5f;     // How far the dash travels in units
    public float dashDuration = 0.5f;
    public float dashCooldown = 0.8f;

    public float DashSpeed
    {
        get { return dashSpeed; }
        set { dashSpeed = value; }
    }
    public float DashDistance
    {
        get { return dashDistance; }
        set { dashDistance = value; }
    }
    public float DashDuration
    {
        get { return dashDuration; }
        set { dashDuration = value; }
    }
    public float DashCooldown
    {
        get { return dashCooldown; }
        set { dashCooldown = value; }
    }



    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 lastAimDir = Vector2.down;

    [Header("Attack Movement")]
    [SerializeField] private float attackMoveMultiplier = 0.3f;

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
        if (!actions) actions = GetComponent<PlayerActions>();

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

        // Dash trigger - ONLY WHILE MOVING
        if (input.DashPressed && dashCooldownTimer <= 0f && !isDashing && move.sqrMagnitude > 0.1f && !actions.IsAttacking)
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
        if (actions != null && actions.IsAttacking)
        {
            Vector2 attackMove = input.Move;
            Vector2 attackMoveDir = attackMove.sqrMagnitude > 0.0001f ? attackMove.normalized : Vector2.zero;
            Vector2 attackTarget = attackMoveDir * moveSpeed * attackMoveMultiplier;
            velocity = Vector2.MoveTowards(velocity, attackTarget, decel * Time.fixedDeltaTime);
            rb.linearVelocity = velocity;
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


