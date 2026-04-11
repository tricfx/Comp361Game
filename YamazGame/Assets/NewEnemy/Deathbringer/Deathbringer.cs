using UnityEngine;

public class Deathbringer : NewEnemy
{
    [Header("Deathbringer")]
    [SerializeField] DeathbringerSpellSpawner spellSpawner;
    [SerializeField] EnemyAttackRange meleeRange;
    [SerializeField] protected AudioClip spellClip;
    [SerializeField] protected float spellVolume = 1f;

    public override void Attack()
    {
        if (!CanAttack) return;

        CanAttack = false;
        animator.SetTrigger(meleeRange.PlayerInRange ? "attack" : "spell");
        Invoke(nameof(ResetAttack), _attackCooldown);
    }

    public void Spell()
    {
        GameObject spell = spellSpawner.SpawnSpell();
        ApplyScalingToSpawnedObject(spell);
    }

    public override void Move(Vector2 startPosition, Vector2 targetPosition)
    {
        Vector2 seek = (targetPosition - startPosition).normalized;
        Vector2 separation = GetSeparationForce();
        Vector2 avoid = GetObstacleAvoidance(seek);
        Vector2 noise = Random.insideUnitCircle * 0.1f;

        Vector2 finalDir = (seek + separation + avoid + noise).normalized;
        rb.AddForce(finalDir * _moveSpeed * Time.fixedDeltaTime);
        FacePlayer(startPosition, targetPosition);
    }

    public override void ResetAttack()
    {
        CanAttack = true;
    }

    // override state handlers, deathbringer has 2 attack range components
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

        if (meleeRange.PlayerInRange)
        {
            Moving = false;
            _currentState = EnemyState.Attack;
            return;
        }

        if (CanAttack)
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
            _searchTimer = _searchDuration;
            _currentState = EnemyState.Search;
            return;
        }

        if (meleeRange.PlayerInRange)
        {
            if (CanAttack)
            {
                FacePlayer(CurrentPosition, meleeRange.PlayerPosition);
                Attack();
            }
            return;
        }

        if (CanAttack)
        {
            FacePlayer(CurrentPosition, attackRange.PlayerPosition);
            Attack();
        }

        _currentState = EnemyState.Chase;
    }

    // deathbringer sprites are flipped by default
    public override void flipDirection(Vector2 direction)
    {
        if (direction.x > 0.1f)
        {
            faceDirection.localScale = new Vector3(-1, 1, 1);
            spriteRenderer.flipX = true;
        }
        else if (direction.x < -0.1f)
        {
            faceDirection.localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
        }
    }

    protected override void FacePlayer(Vector2 enemyPos, Vector2 playerPos)
    {
        float dir = playerPos.x - enemyPos.x;
        if (Mathf.Abs(dir) < 0.1f) return;

        if (dir > 0)
        {
            faceDirection.localScale = new Vector3(-1, 1, 1);
            spriteRenderer.flipX = true;
        }
        else
        {
            faceDirection.localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
        }
    }

    public void PlaySpellSound()
    {
        PlaySound(spellClip, spellVolume);
    }
}
