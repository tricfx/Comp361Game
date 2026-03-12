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
                UnlockMovement();
                if (spriteRenderer)
                {
                    spriteRenderer.color = Color.red;
                    Invoke("ResetColor", 0.1f);
                }
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

    [SerializeField] protected Transform faceDirection;
    [SerializeField] protected EnemyDetectionRange detectionRange;
    [SerializeField] protected EnemyAttackRange attackRange;

    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D feetCollider { get; private set; }

    [SerializeField] protected float _searchDuration = 5f;
    [SerializeField] protected int _maxHealth = 10;
    [SerializeField] protected int _moveSpeed = 2000;
    [SerializeField] protected float _attackCooldown = 1f;
    [SerializeField] protected bool _disableSimulation = false;
    [SerializeField] protected bool _enableInvincibilityWindow = false;
    [SerializeField] protected float _invincibilityLimit = 0.3f;
    [SerializeField] protected LayerMask _obstacleLayer;

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Search
    }

    protected EnemyState _currentState;
    protected Vector2 _lastSeenPlayerPosition;
    protected float _searchTimer = 0f;
    protected int _currentHealth;
    protected bool _targetable = true;
    protected bool _invincible = false;
    protected float _invincibilityTimeElapsed = 0f;
    protected bool _canAttack = true;
    protected bool _canMove = true;
    protected bool _moving = false;
    protected Color originalColor;
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
        originalColor = spriteRenderer.color;

        _currentState = EnemyState.Idle;
    }

    public void Update()
    {
        if (Invincible)
        {
            _invincibilityTimeElapsed += Time.deltaTime;

            if (_invincibilityTimeElapsed > _invincibilityLimit)
            {
                Invincible = false;
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        switch(_currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;
            
            case EnemyState.Attack:
                HandleAttack();
                break;

            case EnemyState.Search:
                HandleSearch();
                break;
        }

        /* if (CanAttack && attackRange.PlayerInRange)
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
        } */
    }

    protected virtual void HandleIdle()
    {
        Moving = false;

        if (Targetable && detectionRange.PlayerInRange)
        {
            _currentState = EnemyState.Chase;
        }
    }

    protected virtual void HandleChase()
    {
        if (detectionRange.PlayerInRange)
        {
            _lastSeenPlayerPosition = detectionRange.PlayerPosition;
        }
        else
        {
            _searchTimer = _searchDuration;
            _currentState = EnemyState.Search;
            return;
        }

        if (attackRange.PlayerInRange && CanAttack)
        {
            Moving = false;
            _currentState = EnemyState.Attack;
            return;
        }

        if (CanMove)
        {
            Moving = true;

            int originalSpeed = _moveSpeed;
            _moveSpeed = Mathf.RoundToInt(_moveSpeed * _slowMultiplier);

            Move(feetCollider.bounds.center, detectionRange.PlayerPosition);

            _moveSpeed = originalSpeed;
        }
    }

    protected virtual void HandleAttack()
    {
        Moving = false;

        if (!attackRange.PlayerInRange)
        {
            _currentState = EnemyState.Chase;
            return;
        }

        if (CanAttack)
        {
            Attack();
        }
    }

    protected virtual void HandleSearch()
    {
        if (detectionRange.PlayerInRange)
        {
            _currentState = EnemyState.Chase;
            return;
        }

        if (_searchTimer <= 0f)
        {
            Moving = false;
            _currentState = EnemyState.Idle;
            return;
        }

        _searchTimer -= Time.fixedDeltaTime;

        if (CanMove)
        {
            Moving = true;
            Move(feetCollider.bounds.center, _lastSeenPlayerPosition);
        }
    }

    protected Vector2 GetSeparationForce(float radius = 1.0f, float strength = 2f)
    {
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(
            feetCollider.bounds.center,
            radius
        );

        Vector2 force = Vector2.zero;

        foreach (Collider2D c in neighbors)
        {
            if (c == feetCollider) continue;
            NewEnemy other = c.GetComponent<NewEnemy>();
            if (other == null) continue;

            Vector2 diff = feetCollider.bounds.center - other.feetCollider.bounds.center;
            float dist = diff.magnitude;

            if (dist > 0)
            {
                force += diff.normalized / dist;
            }
        }

        return force * strength;
    }

    protected Vector2 GetObstacleAvoidance(Vector2 moveDir, float checkDistance = 0.6f)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            feetCollider.bounds.center,
            moveDir,
            checkDistance,
            _obstacleLayer
        );

        if (hit.collider != null)
        {
            Vector2 avoidDir = hit.normal;
            return avoidDir * 2f;
        }

        return Vector2.zero;
    }

    protected void FacePlayer(Vector2 enemyPos, Vector2 playerPos)
    {
        float dir = playerPos.x - enemyPos.x;
        if (Mathf.Abs(dir) < 0.1f) return;

        if (dir > 0)
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
        if (direction.x > 0.1f)
        {
            faceDirection.localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
        }
        else if (direction.x < -0.1f)
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

    private void ResetColor()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public abstract void Attack();
    public abstract void ResetAttack();
    public abstract void Move(Vector2 startPosition, Vector2 targetPosition);
}