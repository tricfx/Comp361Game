using UnityEngine;

public class Deathbringer : NewEnemy
{
    [SerializeField] DeathbringerSpellSpawner spellSpawner;
    [SerializeField] EnemyAttackRange meleeRange;

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
}
