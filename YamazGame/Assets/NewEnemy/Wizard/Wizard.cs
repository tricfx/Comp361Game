using UnityEngine;

public class Wizard : NewEnemy
{
    [SerializeField] WizardProjectileSpawner projectileSpawner;
    [SerializeField] float preferredDistanceMultiplier = 0.8f;
    [SerializeField] float distanceTolerance = 0.3f;
    [SerializeField] float strafeStrength = 0.5f;
    [SerializeField] float noiseStrength = 0.05f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void HandleChase()
    {
        if (detectionRange.PlayerInRange)
        {
            _lastSeenPlayerPosition = detectionRange.PlayerPosition;
        }
        else
        {
            _searchTimer = _searchDuration;
            _currentState = EnemyState.Search;
            return;
        }

        if (attackRange.PlayerInRange && CanAttack)
        {
            Moving = false;
            _currentState = EnemyState.Attack;
            return;
        }

        if (CanMove)
        {
            Moving = true;

            int originalSpeed = _moveSpeed;
            _moveSpeed = Mathf.RoundToInt(_moveSpeed * _slowMultiplier);

            Move(feetCollider.bounds.center, detectionRange.PlayerPosition);

            _moveSpeed = originalSpeed;
        }
        else
        {
            Moving = false;
        }
    }

    protected override void HandleAttack()
    {
        Moving = false;

        if (!detectionRange.PlayerInRange)
        {
            _currentState = EnemyState.Search;
            _searchTimer = _searchDuration;
            return;
        }

        if (CanAttack)
        {
            Attack();
        }

        _currentState = EnemyState.Chase;
    }

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

    public void FireProjectile()
    {
        projectileSpawner.SpawnProjectile();
    }

    public override void Move(Vector2 startPosition, Vector2 targetPosition)
    {
        CircleCollider2D circle = detectionRange.GetComponent<CircleCollider2D>();
        float radius = circle.radius * detectionRange.transform.lossyScale.x;

        float preferredDistance = radius * preferredDistanceMultiplier;

        Vector2 toPlayer = targetPosition - startPosition;
        float distance = toPlayer.magnitude;

        if (distance < 0.001f) return;

        Vector2 seek = toPlayer.normalized;
        Vector2 moveIntent = Vector2.zero;

        if (distance > preferredDistance + distanceTolerance)
        {
            moveIntent += seek;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            moveIntent -= seek;
        }
        else
        {
            Vector2 strafe = new Vector2(-seek.y, seek.x);
            moveIntent += strafe * strafeStrength;
        }

        Vector2 separation = GetSeparationForce();
        Vector2 avoid = GetObstacleAvoidance(moveIntent == Vector2.zero ? seek : moveIntent.normalized);
        Vector2 noise = Random.insideUnitCircle * noiseStrength;

        Vector2 finalDir = (moveIntent + separation + avoid + noise).normalized;
        if (finalDir.sqrMagnitude < 0.001f) return;

        rb.AddForce(finalDir * _moveSpeed * Time.fixedDeltaTime);
        FacePlayer(startPosition, targetPosition);
    }
}
