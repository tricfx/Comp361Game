using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] public int attackDamage = 20;
    [SerializeField] public float attackCooldown = 2f;

    [SerializeField] private BossAnimatorController anim;
    [SerializeField] private BossMovement movement;

    private float lastAttackTime = -Mathf.Infinity;
    public bool IsAttacking { get; private set; } = false;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown && !IsAttacking;

    private void Awake()
    {
        if (!anim) anim = GetComponent<BossAnimatorController>();
        if (!movement) movement = GetComponent<BossMovement>();
    }

    // Called by AI to trigger an attack
    public void PerformAttack(int attackID = 1)
    {
        if (!CanAttack) return;

        IsAttacking = true;
        lastAttackTime = Time.time;
        movement?.Stop();
        anim?.TriggerAttack(attackID);
    }

    // Called by animation event when attack finishes
    public void OnAttackComplete()
    {
        IsAttacking = false;
    }
}
