using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;

    [SerializeField] private BossAnimatorController anim;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 lastFacingDir = Vector2.down;
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<BossAnimatorController>();
    }

    private void FixedUpdate()
    {
        if (!CanMove)
        {
            rb.linearVelocity = Vector2.zero;
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
        CanMove = true; // re-enable movement whenever the AI gives a new direction
        moveDir = dir;
        if (dir.sqrMagnitude > 0.001f)
        {
            lastFacingDir = dir;
            anim?.SetFacing(dir);
            anim?.SetMove(dir);
        }
    }

    public void Stop()
    {
        moveDir = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        anim?.SetSpeed(0f);
        // Note: does NOT set CanMove = false — that's intentional.
        // CanMove is only for external locks (e.g. cutscenes). Stop() just zeroes velocity.
    }
}
