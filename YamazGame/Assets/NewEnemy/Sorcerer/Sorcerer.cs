using System.Collections;
using UnityEngine;

public class Sorcerer : NewEnemy
{
    [SerializeField] SorcererSpellSpawner spellSpawner;
    [SerializeField] float spellTimer = 1f;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpellRoutine());
    }

    IEnumerator SpellRoutine()
    {
        while (true)
        {
            if (!IsAlive)
            {
                yield break;
            }

            if (detectionRange.PlayerInRange)
            {
                GameObject spell = spellSpawner.SpawnSpell();
                ApplyScalingToSpawnedObject(spell);
                yield return new WaitForSeconds(spellTimer);
            }
            else
            {
                yield return null;
            }
        }
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
