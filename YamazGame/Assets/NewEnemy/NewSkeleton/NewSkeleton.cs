using UnityEngine;

public class NewSkeleton : NewEnemy
{
    [SerializeField] float sideApproachMultiplier = 0.8f;
    [SerializeField] float sideBiasStrength = 0.75f;

    public override void Attack()
    {
        if (!CanAttack) return;

        CanAttack = false;
        animator.SetTrigger("attack");
        Invoke(nameof(ResetAttack), _attackCooldown);
    }

    public override void ResetAttack()
    {
        CanAttack = true;
    }

    public override void Move(Vector2 startPosition, Vector2 targetPosition)
    {
        Collider2D attackCollider = attackRange.GetComponent<Collider2D>();

        float sideOffset = 1f;
        if (attackCollider != null)
        {
            sideOffset = attackCollider.bounds.extents.x * sideApproachMultiplier;
        }

        Vector2 rightPoint = targetPosition + new Vector2(sideOffset, 0f);
        Vector2 leftPoint = targetPosition + new Vector2(-sideOffset, 0f);

        float distToRight = Vector2.Distance(startPosition, rightPoint);
        float distToLeft = Vector2.Distance(startPosition, leftPoint);

        Vector2 sideTarget = distToRight < distToLeft ? rightPoint : leftPoint;

        float horizontalOffset = Mathf.Abs(targetPosition.x - startPosition.x);

        float sideBias = 1f - Mathf.Clamp01(horizontalOffset / sideOffset);
        sideBias *= sideBiasStrength;

        Vector2 desiredTarget = Vector2.Lerp(targetPosition, sideTarget, sideBias);

        Vector2 seek = (desiredTarget - startPosition).normalized;
        Vector2 separation = GetSeparationForce();
        Vector2 avoid = GetObstacleAvoidance(seek);
        Vector2 noise = Random.insideUnitCircle * 0.1f;

        Vector2 finalDir = (seek + separation + avoid + noise).normalized;

        if (finalDir.sqrMagnitude < 0.001f) return;

        rb.AddForce(finalDir * _moveSpeed * Time.fixedDeltaTime);
        FacePlayer(startPosition, targetPosition);
    }
}
