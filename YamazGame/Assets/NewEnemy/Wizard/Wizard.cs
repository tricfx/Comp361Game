using UnityEngine;

public class Wizard : NewEnemy
{
    [SerializeField] WizardProjectileSpawner projectileSpawner;

    protected override void Start()
    {
        base.Start();
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
        Vector2 direction = (targetPosition - startPosition).normalized;
        rb.AddForce(direction * _moveSpeed * Time.fixedDeltaTime);
        flipDirection(direction);
    }
}
