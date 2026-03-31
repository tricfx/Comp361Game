using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;

    [SerializeField] private BossAnimatorController anim;

    private Rigidbody2D rb;
    private Vector2 moveDir;
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
        anim?.SetSpeed(moveDir.magnitude);
        anim?.SetMove(moveDir);
    }

    // Called by AI to set movement direction
    public void SetMoveDirection(Vector2 dir)
    {
        moveDir = dir;
        if (dir.sqrMagnitude > 0.001f)
            anim?.SetFacing(dir);
    }

    public void Stop()
    {
        moveDir = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        anim?.SetSpeed(0f);
    }
}
