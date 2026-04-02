using Unity.VisualScripting;
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
            int prevHealth = _currentHealth;
            _currentHealth = value;

            if (_currentHealth < prevHealth)
            {
                animator.SetTrigger("hit");
                if (spriteRenderer)
                {
                    spriteRenderer.color = Color.red;
                    Invoke("ResetColor", 0.1f);
                }

                if (_currentHealth <= 0 && _isAlive)
                {
                    _currentHealth = 0;
                    Die();
                }
                else if (_currentHealth > 0 && _isAlive)
                {
                    UnlockMovement();
                    PlaySound(hurtClip, hurtVolume);
                }
            }
        }
    }

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
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

    public Vector3 CurrentPosition
    {
        get
        {
            return feetCollider.bounds.center;
        }
    }

    [SerializeField] protected Transform faceDirection;
    [SerializeField] protected EnemyDetectionRange detectionRange;
    [SerializeField] protected EnemyAttackRange attackRange;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip moveClip;
    [SerializeField] protected AudioClip attackClip;
    [SerializeField] protected AudioClip hurtClip;
    [SerializeField] protected AudioClip deathClip;
    [SerializeField] protected float moveVolume = 1f;
    [SerializeField] protected float attackVolume = 1f;
    [SerializeField] protected float hurtVolume = 1f;
    [SerializeField] protected float deathVolume = 1f;


    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D feetCollider { get; private set; }

    [SerializeField] protected float _searchDuration = 5f;
    [SerializeField] protected bool _enableIFrames = false;
    [SerializeField] protected float _invincibilityLimit = 0.3f;
    [SerializeField] protected LayerMask _obstacleLayer;

    public int SpawnLevel { get; private set; } = 1;
    public float DamageMultiplier { get; private set; } = 1f;

    private bool _statsInitialized = false;

    [Header("Base Stats (Level 1)")]
    [SerializeField] private int _baseMaxHealth = 30;
    [SerializeField] private int _baseMoveSpeed = 2000;
    [SerializeField] private float _baseAttackCooldown = 1f;

    protected int _maxHealth;
    protected int _moveSpeed;
    protected float _attackCooldown;

    [Header("Level Scaling")]
    [SerializeField] protected float healthScalePerLevel = 0.20f;
    [SerializeField] protected float speedScalePerLevel = 0.05f;
    [SerializeField] protected float cooldownReductionPerLevel = 0.04f;
    [SerializeField] protected float damageScalePerLevel = 0.15f;
    [SerializeField] protected float minAttackCooldown = 0.25f;

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
    protected bool _isAlive = true;
    protected bool _invincible = false;
    protected float _invincibilityTimeElapsed = 0f;
    protected bool _canAttack = true;
    protected bool _canMove = true;
    protected bool _moving = false;
    protected Color originalColor;
    protected float _slowMultiplier = 1f;

   protected bool isagro = false;
    private bool isCountedInAgro = false;
    private SepukuManager sepukuManager;

    public float SlowMultiplier
    {
        get { return _slowMultiplier; }
        set { _slowMultiplier = Mathf.Clamp(value, 0.01f, 1f); }
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        feetCollider = GetComponent<Collider2D>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    protected virtual void Start()
    {
        if (!_statsInitialized)
        {
            InitializeForLevel(1);
        }

        animator.SetBool("alive", true);
        originalColor = spriteRenderer.color;
        _currentState = EnemyState.Idle;
        currentTarget = null;

         sepukuManager = FindFirstObjectByType<SepukuManager>();

        if (!allEnemies.Contains(this))
        {
            allEnemies.Add(this);
        }
    }

    public virtual void InitializeForLevel(int level = 1)
    {
        if (_statsInitialized) return;

        SpawnLevel = Mathf.Max(1, level);

        float levelOffset = SpawnLevel - 1;
        _maxHealth = Mathf.RoundToInt(_baseMaxHealth * (1f + healthScalePerLevel * levelOffset));
        _moveSpeed = Mathf.RoundToInt(_baseMoveSpeed * (1f + speedScalePerLevel * levelOffset));
        _attackCooldown = Mathf.Max(
            minAttackCooldown,
            _baseAttackCooldown * (1f - cooldownReductionPerLevel * levelOffset)
        );

        DamageMultiplier = 1f + damageScalePerLevel * levelOffset;

        CurrentHealth = _maxHealth;

        EnemyHurtbox[] hurtboxes = GetComponentsInChildren<EnemyHurtbox>(true);
        foreach (EnemyHurtbox hurtbox in hurtboxes)
        {
            hurtbox.ApplyDamageMultiplier(DamageMultiplier);
        }

        _statsInitialized = true;
    }

    public void ApplyScalingToSpawnedObject(GameObject spawnedObject)
    {
        EnemyHurtbox hurtbox = spawnedObject.GetComponent<EnemyHurtbox>();
        if (hurtbox != null)
        {
            hurtbox.ApplyDamageMultiplier(DamageMultiplier);
        }
    }

    private void OnDestroy()
    {
        if (allEnemies.Contains(this))
            allEnemies.Remove(this);
        if (isCountedInAgro && sepukuManager != null)
            {
                sepukuManager.RemoveEnnemy();
                isCountedInAgro = false;
            }
    }

    protected virtual void Update()
    {

        HandleAgroCount();
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
    private void HandleAgroCount()
{
    if (sepukuManager == null) return;

    if (isagro && !isCountedInAgro)
    {
        sepukuManager.AddEnnemy();
        isCountedInAgro = true;
    }
    else if (!isagro && isCountedInAgro)
    {
        sepukuManager.RemoveEnnemy();
        isCountedInAgro = false;
    }
}

    protected virtual void FixedUpdate()
    {
        if (!IsAlive)
        {
            Moving = false;
            return;
        }

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
    }

    protected virtual void HandleIdle()
    {
        Moving = false;

        if (IsAlive && detectionRange.PlayerInRange)
        {
            _currentState = EnemyState.Chase;
               isagro = true;
        }
          else
        {
            isagro = false;
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

            Move(CurrentPosition, detectionRange.PlayerPosition);

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
            FacePlayer(CurrentPosition, attackRange.PlayerPosition);
            Attack();
        }
    }

    protected virtual void HandleSearch()
    {
         isagro = true;
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
            Move(CurrentPosition, _lastSeenPlayerPosition);
        }
    }

    protected Vector2 GetSeparationForce(float radius = 1.0f, float strength = 2f)
    {
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(
            CurrentPosition,
            radius
        );

        Vector2 force = Vector2.zero;

        foreach (Collider2D c in neighbors)
        {
            if (c == feetCollider) continue;
            NewEnemy other = c.GetComponent<NewEnemy>();
            if (other == null) continue;

            Vector2 diff = CurrentPosition - other.CurrentPosition;
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
            CurrentPosition,
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

    protected virtual void FacePlayer(Vector2 enemyPos, Vector2 playerPos)
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
            if (!enemy.IsAlive) continue;
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

            if (_enableIFrames)
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

            if (_enableIFrames)
            {
                Invincible = true;
            }
        }
    }

    protected virtual void Die()
    {
        _isAlive = false;
        _canMove = false;
        _canAttack = false;
        _currentState = EnemyState.Idle;

        Moving = false;

        PlaySound(deathClip, deathVolume);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        if (feetCollider != null)
        {
            feetCollider.enabled = false;
        }

        if (detectionRange != null)
        {
            Collider2D detectionCollider = detectionRange.GetComponent<Collider2D>();
            if (detectionCollider != null)
            {
                detectionCollider.enabled = false;
            }
        }

        if (attackRange != null)
        {
            Collider2D attackCollider = attackRange.GetComponent<Collider2D>();
            if (attackCollider != null)
            {
                attackCollider.enabled = false;
            }
        }

        animator.SetBool("alive", false);
    }

    public void TakeKnockback(Vector2 knockback)
    {
        knockback.y *= 0.5f;
        knockback.Normalize();
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }

    public virtual void flipDirection(Vector2 direction)
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
        if (!IsAlive) return;
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

    protected void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayAttackSound()
    {
        PlaySound(attackClip, attackVolume);
    }

    public abstract void Attack();
    public abstract void ResetAttack();
    public abstract void Move(Vector2 startPosition, Vector2 targetPosition);
}