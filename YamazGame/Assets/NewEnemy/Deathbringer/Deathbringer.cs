using UnityEngine;

public class Deathbringer : NewEnemy
{
    [SerializeField] DeathbringerSpellSpawner spellSpawner;

    public override void Attack()
    {
        
    }

    public override void Move(Vector2 startPosition, Vector2 targetPosition)
    {
        
    }

    public override void ResetAttack()
    {
        CanAttack = true;
    }
}
