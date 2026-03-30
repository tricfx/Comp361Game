using UnityEngine;

public class ImperialGuard : NewEnemy
{
    [SerializeField] float dashSpeed = 50f;
    [SerializeField] float dashDistanceMultiplier = 1.5f;
    [SerializeField] float orbitNoiseStrength = 0.05f;
    bool isDashing = false;
    Vector2 dashTarget;
    Vector2 committedDashTarget;
    float orbitSign;

    protected override void Start()
    {
        base.Start();
        orbitSign = Random.Range(0,2) == 0 ? -1f : 1f;
    }

    protected override void FixedUpdate()
    {
        if (isDashing)
        {
            Move(feetCollider.bounds.center, committedDashTarget);
            return;
        }

        base.FixedUpdate();
    }

    public override void Attack()
    {
        if (!CanAttack) return;

        committedDashTarget = detectionRange.PlayerInRange
            ? detectionRange.PlayerPosition
            : _lastSeenPlayerPosition;

        CanAttack = false;
        animator.SetTrigger("attack");
        Invoke(nameof(ResetAttack), _attackCooldown);
    }

    public override void Move(Vector2 startPosition, Vector2 targetPosition)
    {
        if (isDashing)
        {
            Vector2 newPos = Vector2.MoveTowards(
                rb.position,
                dashTarget,
                dashSpeed * Time.fixedDeltaTime
            );

            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, dashTarget) < 0.05f)
            {
                isDashing = false;
            }

            return;
        }

        HoverAtSide(startPosition, targetPosition);
    }

    public void HoverAtSide(Vector2 startPosition, Vector2 targetPosition)
    {
        CircleCollider2D circle = detectionRange.GetComponent<CircleCollider2D>();
        float radius = circle.radius * detectionRange.transform.lossyScale.x;

        float desiredDistance = radius * 0.9f;
        float tolerance = 0.2f;

        Vector2 rightPoint = targetPosition + new Vector2(desiredDistance, 0f);
        Vector2 leftPoint  = targetPosition + new Vector2(-desiredDistance, 0f);

        float distToRight = Vector2.Distance(startPosition, rightPoint);
        float distToLeft  = Vector2.Distance(startPosition, leftPoint);

        Vector2 desiredPoint = distToRight < distToLeft ? rightPoint : leftPoint;

        Vector2 toDesired = desiredPoint - startPosition;
        float distanceToDesired = toDesired.magnitude;

        if (distanceToDesired < 0.001f)
            return;

        Vector2 seek = toDesired.normalized;

        Vector2 toPlayer = targetPosition - startPosition;
        float playerDistance = toPlayer.magnitude;

        Vector2 radialCorrection = Vector2.zero;
        if (playerDistance > desiredDistance + tolerance)
        {
            radialCorrection += toPlayer.normalized;
        }
        else if (playerDistance < desiredDistance - tolerance)
        {
            radialCorrection -= toPlayer.normalized;
        }

        Vector2 separation = GetSeparationForce();
        Vector2 avoid = GetObstacleAvoidance(seek);
        Vector2 noise = Random.insideUnitCircle * orbitNoiseStrength;

        Vector2 finalDir = (seek + radialCorrection + separation + avoid + noise).normalized;

        if (finalDir.sqrMagnitude < 0.001f)
            return;

        rb.AddForce(finalDir * _moveSpeed * Time.fixedDeltaTime);
        FacePlayer(startPosition, targetPosition);
    }

    public override void ResetAttack()
    {
        CanAttack = true;
    }

    public void DashTowardPlayer()
    {
        Vector2 enemyPos = feetCollider.bounds.center;
        Vector2 playerPos = committedDashTarget;

        Vector2 dir = (playerPos - enemyPos);
        if (dir.sqrMagnitude < 0.001f) return;

        dir.Normalize();

        float distance = Vector2.Distance(enemyPos, playerPos);
        float dashDistance = distance * dashDistanceMultiplier;

        dashTarget = enemyPos + dir * dashDistance;
        isDashing = true;

        rb.linearVelocity = Vector2.zero;
        flipDirection(dir);
        PlayAttackSound();
    }
}
