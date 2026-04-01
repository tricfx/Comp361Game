using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    public int AttackDamage
    {
        get
        {
            return _attackDamage;
        }
    }

    public int KnockbackForce
    {
        get
        {
            return _knockbackForce;
        }
    }

    [Header("Base Stats (Level 1)")]
    [SerializeField] private int _baseAttackDamage = 2;
    [SerializeField] private int _knockbackForce = 10;
    
    private int _attackDamage;
    private Collider2D hurtbox;

    void Awake()
    {
        _attackDamage = _baseAttackDamage;
    }

    void Start()
    {
        hurtbox = GetComponent<Collider2D>();
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        _attackDamage = Mathf.Max(1, Mathf.RoundToInt(_baseAttackDamage * multiplier));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        NewEnemy attacker = GetComponentInParent<NewEnemy>();
        if (attacker == null) return;

        // Damage player
        if (other.CompareTag("PlayerHitbox"))
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player == null) return;

            player.TakeDamage(_attackDamage);
            return;
        }

        // Damage enemy if on opposite team (for charm fights)
        NewEnemy targetEnemy = other.GetComponentInParent<NewEnemy>();

        if (targetEnemy != null && targetEnemy != attacker && targetEnemy.IsAlive && targetEnemy.CurrentTeam != attacker.CurrentTeam)
        {
            targetEnemy.TakeDamage(_attackDamage);
        }
    }
}
