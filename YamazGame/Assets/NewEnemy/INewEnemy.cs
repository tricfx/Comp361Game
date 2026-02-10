using UnityEngine;

public interface INewEnemy {
    
    float MaxHealth { set; get; }
    float CurrentHealth { set; get; }
    bool Targetable { set; get; }
    bool Invincible { set; get; }

    bool CanAttack { set; get; }
    bool CanMove { set; get; }
    bool Moving { set; get; }

    void Start();
    void Update();
    void FixedUpdate();
    void OnObjectDestroyed();

    void Attack();
    void ResetAttack();
    void TakeDamage(float damage);
    void TakeKnockback(Vector2 knockback);

    void Move(Vector2 startPosition, Vector2 targetPosition);
    void LockMovement();
    void UnlockMovement();

}