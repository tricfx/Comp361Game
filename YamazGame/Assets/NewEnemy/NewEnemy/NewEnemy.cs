using UnityEngine;

public abstract class NewEnemy : MonoBehaviour, INewEnemy
{
    public enum Team
    {
        Ally,
        Enemy
    }

    public Team CurrentTeam = Team.Enemy;

    private bool _isCharmed = false;
    private float _charmTimer = 0f;

    public static System.Collections.Generic.List<NewEnemy> allEnemies = new System.Collections.Generic.List<NewEnemy>();

    protected Transform currentTarget;
    public Transform CurrentTarget => currentTarget;
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
        if (!allEnemies.Contains(this))
            allEnemies.Add(this);

        currentTarget = null;
    }

    private void OnDestroy()
    {
        if (allEnemies.Contains(this))
            allEnemies.Remove(this);
    }

    public void Update()
    {
        if (_isCharmed)
        {
            _charmTimer -= Time.deltaTime;

            if (_charmTimer <= 0f)
            {
                _isCharmed = false;
                CurrentTeam = Team.Enemy;
                currentTarget = null;
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
        if (!Targetable) return;

        Vector2 targetPosition;

        if (_isCharmed)
        {
            if (currentTarget == null || !(currentTarget is object && currentTarget.GetComponent<NewEnemy>() != null && currentTarget.GetComponent<NewEnemy>().Targetable))
            {
                // Clear invalid or dead target
                currentTarget = null;

                NewEnemy newTarget = FindClosestEnemy();
                if (newTarget != null)
                    currentTarget = newTarget.transform;
            }

            if (currentTarget == null)
            {
                Moving = false;
                return;
            }

            targetPosition = currentTarget.position;

            // Attack using the same behaviour used against the player
            float attackDistance = Vector2.Distance(transform.position, targetPosition);

            if (CanAttack && attackDistance < 6f)
            {
                Moving = false;
                Attack();
                return;
            }
        }
        else
        {
            NewEnemy allyTarget = FindClosestEnemy();

            if (allyTarget != null && allyTarget.CurrentTeam != CurrentTeam)
            {
                targetPosition = allyTarget.transform.position;

                if (CanAttack && Vector2.Distance(transform.position, targetPosition) < 1.5f)
                {
                    Moving = false;
                    Attack();
                    return;
                }
            }
            else
            {
                if (CanAttack && attackRange.PlayerInRange)
                {
                    Moving = false;
                    Attack();
                    return;
                }

                if (!detectionRange.PlayerInRange)
                {
                    Moving = false;
                    return;
                }

                targetPosition = detectionRange.PlayerPosition;
            }
        }

        if (CanMove)
        {
            Moving = true;
            int originalSpeed = _moveSpeed;
            _moveSpeed = Mathf.RoundToInt(_moveSpeed * _slowMultiplier);
            Move(transform.position, targetPosition);
            _moveSpeed = originalSpeed;
        }
        else
        {
            Moving = false;
        }
    }
    
    public void ApplyCharm(float duration)
    {
        // Always reset charm state
        _isCharmed = true;
        _charmTimer = duration;

        // Put the enemy onto the Ally team again
        CurrentTeam = Team.Ally;

        // Clear any previous target
        currentTarget = null;

        // Pick a fresh enemy target
        NewEnemy targetEnemy = FindClosestEnemy();
        if (targetEnemy != null)
            currentTarget = targetEnemy.transform;
    }

    protected NewEnemy FindClosestEnemy()
    {
        float closest = Mathf.Infinity;
        NewEnemy best = null;

        foreach (var enemy in allEnemies)
        {
            if (enemy == this) continue;
            if (!enemy.Targetable) continue;
            if (enemy.CurrentTeam == this.CurrentTeam) continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);

            if (dist < closest)
            {
                closest = dist;
                best = enemy;
            }
        }

        return best;
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
        float sign = direction.x > 0 ? 1f : -1f;

        faceDirection.localScale = new Vector3(sign, 1, 1);
        spriteRenderer.flipX = sign < 0;

        // Ensure hitbox flips with the enemy so melee attacks work on both sides
        if (hitbox != null)
        {
            Vector3 scale = hitbox.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * sign;
            hitbox.transform.localScale = scale;
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

    public abstract void Attack();
    public abstract void ResetAttack();
    public abstract void Move(Vector2 startPosition, Vector2 targetPosition);
}