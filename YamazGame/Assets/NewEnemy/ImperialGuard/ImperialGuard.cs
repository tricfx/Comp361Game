using UnityEngine;

public class ImperialGuard : NewEnemy
{
    [SerializeField] float dashSpeed = 50f;
    [SerializeField] float dashDistanceMultiplier = 1.5f;
    bool isDashing = false;
    Vector2 dashTarget;
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
            Move(feetCollider.bounds.center, detectionRange.PlayerPosition);
            return;
        }

        base.FixedUpdate();
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

        OrbitPlayer(startPosition, targetPosition);
    }

    public void OrbitPlayer(Vector2 startPosition, Vector2 targetPosition) {
        CircleCollider2D circle = detectionRange.GetComponent<CircleCollider2D>();
        float radius = circle.radius * detectionRange.transform.lossyScale.x;

        float desiredDistance = radius * 0.9f; // stay just inside
        float tolerance = 0.2f;

        Vector2 toPlayer = targetPosition - startPosition;
        float distance = toPlayer.magnitude;

        if (distance < 0.001f) return;

        Vector2 radialDir = toPlayer.normalized;
        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x) * orbitSign;
        Vector2 moveDir = Vector2.zero;

        if (distance > desiredDistance)
        {
            moveDir += radialDir;
        }
        else if (distance < desiredDistance - tolerance)
        {
            moveDir -= radialDir;
        }

        moveDir += tangentDir;
        moveDir.Normalize();

        rb.AddForce(moveDir * _moveSpeed * Time.fixedDeltaTime);
        flipDirection(moveDir);
    }

    public override void ResetAttack()
    {
        CanAttack = true;
    }

    public void DashTowardPlayer()
    {
        if (!detectionRange.PlayerInRange) return;

        Vector2 enemyPos = feetCollider.bounds.center;
        Vector2 playerPos = detectionRange.PlayerPosition;
        Vector2 dir = (playerPos - enemyPos).normalized;

        float distance = Mathf.Abs(Vector2.Distance(enemyPos, playerPos));
        float dashDistance = distance * dashDistanceMultiplier;

        dashTarget = enemyPos + dir * dashDistance;
        isDashing = true;

        rb.linearVelocity = Vector2.zero;
        flipDirection(dir);
    }
}
