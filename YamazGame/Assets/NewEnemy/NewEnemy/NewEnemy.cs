using UnityEngine;

public abstract class NewEnemy : MonoBehaviour, INewEnemy
{
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    public int CurrentHealth
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

    [SerializeField] protected EnemyHitbox hitbox;
    [SerializeField] protected EnemyHurtbox hurtbox;
    [SerializeField] protected Transform faceDirection;
    [SerializeField] protected EnemyDetectionRange detectionRange;
    [SerializeField] protected EnemyAttackRange attackRange;
    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D feetCollider { get; private set; }

    [SerializeField] protected int _maxHealth = 10;
    [SerializeField] protected int _moveSpeed = 2000;
    [SerializeField] protected float _attackCooldown = 1f;
    [SerializeField] protected bool _disableSimulation = false;
    [SerializeField] protected bool _enableInvincibilityWindow = false;
    [SerializeField] protected float _invincibilityLimit = 0.3f;

    protected int _currentHealth;
    protected bool _targetable = true;
    protected bool _invincible = false;
    protected float _invincibilityTimeElapsed = 0f;
    protected bool _canAttack = true;
    protected bool _canMove = true;
    protected bool _moving = false;
    protected float _slowMultiplier = 1f;

    private bool _isCharmed = false;
    private float _charmTimer = 0f;

    public float SlowMultiplier
    {
        get { return _slowMultiplier; }
        set { _slowMultiplier = Mathf.Clamp(value, 0.01f, 1f); }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        feetCollider = GetComponent<Collider2D>();

        CurrentHealth = MaxHealth;
        animator.SetBool("alive", true);
    }

    public void Update()
    {
        // Handle charm timer
        if (_isCharmed)
        {
            _charmTimer -= Time.deltaTime;

            if (_charmTimer <= 0f)
            {
                _isCharmed = false;
            }
        }

        if (Invincible)
        {
            _invincibilityTimeElapsed += Time.deltaTime;

            if (_invincibilityTimeElapsed > _invincibilityLimit)
            {
                Invincible = false;
            }
        }
    }

    public void FixedUpdate()
    {
        // If charmed, target other enemies instead of the player
        if (_isCharmed)
        {
            NewEnemy[] enemies = FindObjectsOfType<NewEnemy>();

            NewEnemy targetEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (NewEnemy enemy in enemies)
            {
                if (enemy == this) continue;
                if (enemy._isCharmed) continue;
                if (!enemy.Targetable) continue;

                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetEnemy = enemy;
                }
            }

            if (targetEnemy != null)
            {
                Moving = true;
                int originalSpeed = _moveSpeed;
                _moveSpeed = Mathf.RoundToInt(_moveSpeed * _slowMultiplier);
                Move(transform.position, targetEnemy.transform.position);
                _moveSpeed = originalSpeed;
            }
            else
            {
                Moving = false;
            }

            return;
        }

        if (CanAttack && attackRange.PlayerInRange)
        {
            Moving = false;
            Attack();
        }
        else if (CanMove && Targetable && detectionRange.PlayerInRange)
        {
            Moving = true;
            int originalSpeed = _moveSpeed;
            _moveSpeed = Mathf.RoundToInt(_moveSpeed * _slowMultiplier);
            Move(transform.position, detectionRange.PlayerPosition);
            _moveSpeed = originalSpeed;
        }
        else
        {
            Moving = false;
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
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

    public void TakeDamage(int damage)
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
        knockback.y *= 0.5f;
        knockback.Normalize();
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }

    public void flipDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            faceDirection.localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
        }
        else
        {
            faceDirection.localScale = new Vector3(-1, 1, 1);
            spriteRenderer.flipX = true;
        }
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

    public void ApplyCharm(float duration)
    {
        _isCharmed = true;
        _charmTimer = duration;
    }

    public abstract void Attack();
    public abstract void ResetAttack();
    public abstract void Move(Vector2 startPosition, Vector2 targetPosition);
}