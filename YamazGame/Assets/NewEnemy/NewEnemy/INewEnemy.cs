using UnityEngine;

public interface INewEnemy {
    
    int MaxHealth { set; get; }
    int CurrentHealth { set; get; }
    bool Targetable { set; get; }
    bool Invincible { set; get; }

    bool CanAttack { set; get; }
    bool CanMove { set; get; }
    bool Moving { set; get; }

    void Attack();
    void ResetAttack();
    void TakeDamage(int damage, Vector2 knockback);
    void TakeDamage(int damage);
    void TakeKnockback(Vector2 knockback);
    void flipDirection(Vector2 direction);

    void Move(Vector2 startPosition, Vector2 targetPosition);
    void LockMovement();
    void UnlockMovement();

}