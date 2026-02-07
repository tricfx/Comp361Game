using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;                // Base movement speed
    public float runSpeed = 8f;             // Running speed
    public float dashSpeed = 12f;           // Dash speed
    public float dashDuration = 0.2f;       // How long dash lasts
    public float dashCooldown = 1f;         // Cooldown before next dash

    [Header("Player Stats")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Ability Placeholders")]
    public GameObject abilityQ;
    public GameObject abilityE;
    // tune these in inspector if cooldowns feel wrong
    [Tooltip("Cooldown in seconds for Q ability")]
    public float abilityQCooldown = 3f;
    [Tooltip("Cooldown in seconds for E ability")]
    public float abilityECooldown = 5f;

    [Header("Animation & Visuals")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Attack Settings")]
    public float attackRange = 1f;          // Attack collision range
    public int attackDamage = 10;           // Base attack damage
    public Transform attackPoint;           // Empty GameObject to mark attack origin
    public LayerMask enemyLayers;           // What layers count as enemies

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMovement;           // Store last direction for attacks
    private bool isDashing = false;
    private bool isRunning = false;
    private bool isRunningNorth = false;
    private bool isRunningSouth = false;
    private bool isRunningEast = false;
    private bool isRunningWest = false;
    private bool isFacingNorth = false;
    private bool isFacingSouth = false;
    private bool isFacingEast = false;
    private bool isFacingWest = false;
    private float dashTime = 0f;
    private float lastDash = -Mathf.Infinity;
    // timestamps for Q/E so we can show cooldown on HUD
    private float lastAbilityQ = -Mathf.Infinity;
    private float lastAbilityE = -Mathf.Infinity;

    // Cached animator param names (avoids errors when params don't exist)
    private System.Collections.Generic.HashSet<string> animParams;


    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
        set
        {
            isRunning = value;
            if (HasAnimParam("isRunning")) animator.SetBool("isRunning", isRunning);
        }
    }
    public bool IsRunningNorth
    {
        get
        {
            return isRunningNorth;
        }
        set
        {
            isRunningNorth = value;
            if (HasAnimParam("isRunningNorth")) animator.SetBool("isRunningNorth", isRunningNorth);
        }
    }
    public bool IsRunningSouth
    {
        get
        {
            return isRunningSouth;
        }
        set
        {
            isRunningSouth = value;
            if (HasAnimParam("isRunningSouth")) animator.SetBool("isRunningSouth", isRunningSouth);
        }
    }
    public bool IsRunningEast
    {
        get
        {
            return isRunningEast;
        }
        set
        {
            isRunningEast = value;
            if (HasAnimParam("isRunningEast")) animator.SetBool("isRunningEast", isRunningEast);
        }
    }

    public bool IsRunningWest
    {
        get { return isRunningWest; }
        set
        {
            isRunningWest = value;
            if (HasAnimParam("isRunningWest")) animator.SetBool("isRunningWest", isRunningWest);
        }
    }

    public bool IsFacingNorth { get => isFacingNorth; set => isFacingNorth = value; }
    public bool IsFacingSouth { get => isFacingSouth; set => isFacingSouth = value; }
    public bool IsFacingEast { get => isFacingEast; set => isFacingEast = value; }
    public bool IsFacingWest { get => isFacingWest; set => isFacingWest = value; }

    //  Property for seconds before next dash 0 means on cooldown, 1 means ready
    public float DashCooldownNormalized
    {
        get
        {
            float remaining = dashCooldown - (Time.time - lastDash);
            if (remaining <= 0f) return 1f;
            return 1f - (remaining / dashCooldown);
        }
    }

    // 1 = ready, 0 = on cooldown (HUD reads these)
    public float AbilityQCooldownNormalized
    {
        get
        {
            float remaining = abilityQCooldown - (Time.time - lastAbilityQ);
            if (remaining <= 0f) return 1f;
            return 1f - (remaining / abilityQCooldown);
        }
    }

    public float AbilityECooldownNormalized
    {
        get
        {
            float remaining = abilityECooldown - (Time.time - lastAbilityE);
            if (remaining <= 0f) return 1f;
            return 1f - (remaining / abilityECooldown);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;

        // Auto-grab components if not assigned
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Cache which animator parameters exist (avoids "parameter does not exist" errors)
        animParams = new System.Collections.Generic.HashSet<string>();
        if (animator != null)
        {
            foreach (var p in animator.parameters)
                animParams.Add(p.name);
        }
    }

    bool HasAnimParam(string name) => animator != null && animParams != null && animParams.Contains(name);

    void Update()
    {
        // --- Input ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // Store last movement for attack direction
        if (movement != Vector2.zero)
        {
            lastMovement = movement;
        }

        if (movement != Vector2.zero)
        {
            IsRunning = true;
        }
        if (movement == Vector2.zero)
        {
            IsRunning = false;
        }


        // Update running direction animations ONLY when running AND moving
        if (IsRunning)
        {
            // Determine pure directions
            bool pureVertical = Mathf.Approximately(movement.x, 0f);
            bool pureHorizontal = Mathf.Approximately(movement.y, 0f);

            // Reset ALL first (critical for instant switching)
            IsRunningNorth = false;
            IsRunningSouth = false;
            IsRunningEast = false;
            IsRunningWest = false;

            // Set the current direction
            if (pureVertical && movement.y > 0)
            {
                IsRunningNorth = true;
            }
            else if (pureVertical && movement.y < 0)
            {
                IsRunningSouth = true;
            }
            else if (pureHorizontal && movement.x > 0)
            {
                IsRunningEast = true;

            }
            else if (pureHorizontal && movement.x < 0)
            {
                IsRunningWest = true;
            }

        }
        else
        {
            IsRunningNorth = false;
            IsRunningSouth = false;
            IsRunningEast = false;
            IsRunningWest = false;
        }

        if (!IsRunning)
        { }





        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time >= lastDash + dashCooldown)
        {
            StartDash();
        }

        // --- Ability placeholders ---
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseAbilityQ();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseAbilityE();
        }

        // --- Attack stub ---
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // --- Animation Updates (placeholder for when animator is added) ---
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = movement * dashSpeed;
        }
        else
        {
            // Only use run speed if BOTH running AND moving
            float currentSpeed = (isRunning && movement != Vector2.zero) ? runSpeed : speed;
            rb.linearVelocity = movement * currentSpeed;
        }
    }

    // --- Dash Logic ---
    void StartDash()
    {
        if (movement == Vector2.zero) return; // can't dash without direction
        isDashing = true;
        dashTime = dashDuration;
        lastDash = Time.time;

        // Trigger dash animation in animator if it exists
        if (animator != null && HasAnimParam("Dash"))
            animator.SetTrigger("Dash");
    }


    void LateUpdate()
    {
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false;
            }
        }
    }

    // --- Attack stub ---
    void Attack()
    {
        Debug.Log("Attack triggered");

        // Trigger attack animation when animator is ready
        if (animator != null && HasAnimParam("Attack"))
        {
            animator.SetTrigger("Attack");
        }

        // Detect enemies in range (when attackPoint is assigned)
        if (attackPoint != null)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                // Call TakeDamage on enemy (adjust to match your enemy script)
                // enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                Debug.Log("Hit " + enemy.name);
            }
        }
    }

    // --- Ability stubs (with cooldown so HUD can show it) ---
    void UseAbilityQ()
    {
        if (Time.time < lastAbilityQ + abilityQCooldown) return;

        lastAbilityQ = Time.time;
        Debug.Log("Ability Q triggered");

        if (abilityQ != null)
        {
            Instantiate(abilityQ, transform.position, Quaternion.identity);
        }
    }

    void UseAbilityE()
    {
        if (Time.time < lastAbilityE + abilityECooldown) return;

        lastAbilityE = Time.time;
        Debug.Log("Ability E triggered");

        if (abilityE != null)
        {
            Instantiate(abilityE, transform.position, Quaternion.identity);
        }
    }

    // --- Animation Updates ---
    void UpdateAnimations()
    {
        if (animator == null) return;

        // Only play run animation when actually moving (Shift + WASD)
        if (HasAnimParam("isRunning"))
            animator.SetBool("isRunning", isRunning && movement != Vector2.zero);

        // Set movement parameters for blend trees (only if they exist in the Animator)
        if (HasAnimParam("MoveX")) animator.SetFloat("MoveX", movement.x);
        if (HasAnimParam("MoveY")) animator.SetFloat("MoveY", movement.y);
        if (HasAnimParam("Speed")) animator.SetFloat("Speed", movement.magnitude);
        if (HasAnimParam("LastMoveX")) animator.SetFloat("LastMoveX", lastMovement.x);
        if (HasAnimParam("LastMoveY")) animator.SetFloat("LastMoveY", lastMovement.y);
    }

    // --- Take Damage method ---
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log("Player HP: " + currentHP);

        // Trigger hurt animation when animator is ready
        if (animator != null && HasAnimParam("Hurt"))
        {
            animator.SetTrigger("Hurt");
        }

        // Check for death
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died");

        if (animator != null && HasAnimParam("Death"))
        {
            animator.SetTrigger("Death");
        }

        // Disable player controls or reload scene
        // For now just disable the script
        this.enabled = false;
    }

    // --- Collision Detection ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Example: if player hits enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // TakeDamage(collision.gameObject.GetComponent<Enemy>().damage);
            Debug.Log("Collided with enemy");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Example: if player enters damage zone
        if (other.CompareTag("DamageZone"))
        {
            TakeDamage(10);
        }
    }

    // --- Gizmos for attack range visualization in editor ---
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

