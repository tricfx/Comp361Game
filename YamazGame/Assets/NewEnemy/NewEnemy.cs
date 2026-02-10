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

    protected EnemyHitbox hitbox;
    protected EnemyHurtbox hurtbox;
    protected EnemyDetectionRange detectionRange;
    protected EnemyAttackRange attackRange;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Collider2D feetCollider;

    [SerializeField] protected float _maxHealth = 10f;
    [SerializeField] protected float _moveSpeed = 500f;
    [SerializeField] protected float _attackCooldown = 1f;
    [SerializeField] protected bool _disableSimulation = false;
    [SerializeField] protected bool _enableInvincibilityWindow = false;
    [SerializeField] protected float _invincibilityLimit = 0.3f;

    protected float _currentHealth;
    protected bool _targetable = true;
    protected bool _invincible = false;
    protected float _invincibilityTimeElapsed = 0f;
    protected bool _canAttack = true;
    protected bool _canMove = true;
    protected bool _moving = false;

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