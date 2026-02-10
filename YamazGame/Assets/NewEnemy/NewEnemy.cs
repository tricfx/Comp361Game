using UnityEngine;

public abstract class NewEnemy : MonoBehaviour, INewEnemy
{
    public float MaxHealth {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            if (value < _currentHealth)
            {
                animator.SetTrigger("hit");
            }

            _currentHealth = value;

            if (_currentHealth <= 0)
            {
                animator.SetBool("alive", false);
                Targetable = false;
            }
        }
    }

    public bool Targetable
    {
        get
        {
            return _targetable;
        }
        set
        {
            _targetable = value;
            if (_disableSimulation)
            {
                rb.simulated = false;
            }
            feetCollider.enabled = value;
        }
    }

    public bool Invincible
    {
        get
        {
            return _invincible;
        }
        set
        {
            _invincible = value;
            if (_invincible == true)
            {
                _invincibilityTimeElapsed = 0f;
            }
        }
    }

    public bool CanAttack
    {
        get
        {
            return _canAttack;
        }
        set
        {
            _canAttack = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return _canMove;
        }
        set
        {
            _canMove = value;
        }
    }

    public bool Moving
    {
        get
        {
            return _moving;
        }
        set
        {
            _moving = value;
            animator.SetBool("moving", _moving);
        }
    }

    EnemyHitbox hitbox;
    EnemyHurtbox hurtbox;
    EnemyDetectionRange detectionRange;
    EnemyAttackRange attackRange;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;
    Collider2D feetCollider;

    [SerializeField] float _maxHealth = 10f;
    [SerializeField] float _attackDamage = 1f;
    [SerializeField] float _knockbackForce = 15f;
    [SerializeField] float _moveSpeed = 500f;
    [SerializeField] float _attackCooldown = 1f;
    [SerializeField] bool _disableSimulation = false;
    [SerializeField] bool _enableInvincibilityWindow = false;
    [SerializeField] float _invincibilityLimit = 0.3f;

    float _currentHealth;
    bool _targetable = true;
    bool _invincible = false;
    float _invincibilityTimeElapsed = 0f;
    bool _canAttack = true;
    bool _canMove = true;
    bool _moving = false;

    public void Start()
    {
        CurrentHealth = MaxHealth;
        animator.SetBool("alive", true);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        feetCollider = GetComponent<Collider2D>();

        hitbox = transform.Find("Hitbox").GetComponent<EnemyHitbox>();
        hurtbox = transform.Find("Hurtbox").GetComponent<EnemyHurtbox>();
        detectionRange = transform.Find("DetectionRange").GetComponent<EnemyDetectionRange>();
        attackRange = transform.Find("AttackRange").GetComponent<EnemyAttackRange>();
    }

    public void FixedUpdate()
    {
        if (Invincible)
        {
            _invincibilityTimeElapsed += Time.deltaTime;

            if (_invincibilityTimeElapsed > _invincibilityLimit)
            {
                Invincible = false;
            }
        }

        if (CanMove && Targetable && detectionRange.PlayerInRange)
        {
            Move(gameObject.transform.position, detectionRange.PlayerPosition);
        }

        //////////// *** //////////////
    }

    public void TakeDamage(float damage, Vector2 knockback)
    {
        if (!Invincible)
        {
            CurrentHealth -= damage;
            TakeKnockback(knockback);

            if (_enableInvincibilityWindow)
            {
                Invincible = true;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!Invincible)
        {
            CurrentHealth -= damage;

            if (_enableInvincibilityWindow)
            {
                Invincible = true;
            }
        }
    }

    public void TakeKnockback(Vector2 knockback)
    {
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }

    public void LockMovement()
    {
        CanMove = false;
    }

    public void UnlockMovement()
    {
        CanMove = true;
    }

    public void OnObjectDestroyed()
    {
        Destroy(gameObject);
    }

    public abstract void Attack();
    public abstract void ResetAttack();
    public abstract void Move(Vector2 startPosition, Vector2 targetPosition);
}