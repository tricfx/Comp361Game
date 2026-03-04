using UnityEngine;

public class ImperialGuard : NewEnemy
{
    float orbitSign;

    protected override void Start()
    {
        base.Start();
        orbitSign = Random.Range(0,2) == 0 ? -1f : 1f;
    }

    public override void Attack()
    {
        if (!CanAttack) return;

        CanAttack = false;

        animator.SetTrigger("attack");

        Invoke(nameof(ResetAttack), _attackCooldown);
    }

    public override void Move(Vector2 startPosition, Vector2 targetPosition)
    {
        // radius of detection range
        CircleCollider2D circle = detectionRange.GetComponent<CircleCollider2D>();
        float radius = circle.radius * detectionRange.transform.lossyScale.x;

        float desiredDistance = radius * 0.9f; // stay just inside
        float tolerance = 0.2f;

        Vector2 toPlayer = targetPosition - startPosition;
        float distance = toPlayer.magnitude;

        if (distance < 0.001f) return;

        Vector2 radialDir = toPlayer.normalized;

        // perpendicular direction for circling
        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x) * orbitSign;
        
        Vector2 moveDir = Vector2.zero;

        // --- radial control ---
        if (distance > desiredDistance)
        {
            // too far (near edge) → move inward slightly
            moveDir += radialDir;
        }
        else if (distance < desiredDistance - tolerance)
        {
            // too close → move outward
            moveDir -= radialDir;
        }

        // --- sideways orbit ---
        moveDir += tangentDir;

        moveDir.Normalize();

        rb.AddForce(moveDir * _moveSpeed * Time.fixedDeltaTime);
        flipDirection(moveDir);
    }

    public override void ResetAttack()
    {
        CanAttack = true;
    }
}
