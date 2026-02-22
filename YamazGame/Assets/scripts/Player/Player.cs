using System.Collections.Generic;
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

    private List<string> activeBuffs = new List<string>();

    [Header("Ability Placeholders")]
    public IAbility abilityQ;
    public IAbility abilityE;
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
    [Tooltip("How long movement is locked during attack (match your attack animation length)")]
    public float attackDuration = 1.3f;      // How long attack state lasts — movement disabled this whole time
    public Transform attackPoint;         // Empty GameObject to mark attack origin
    public LayerMask enemyLayers;          // What layers count as enemies

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMovement;           // Store last direction for attacks
    private bool isDashing = false;
    private bool isRunning = false;
    private bool isRunningNorth = false;
    private bool isRunningSouth = false;
    private bool isRunningEast = false;
    private bool isRunningWest = false;
    private bool isIdleEast = false;
    private bool isIdleWest = false;
    private bool isIdleNorth = false;
    private bool isIdleSouth = false;

    private bool isAttacking = false;
    private bool isAttackingEast = false;
    private bool isAttackingWest = false;
    private float dashTime = 0f;
    private float lastDash = -Mathf.Infinity;
    // timestamps for Q/E so we can show cooldown on HUD
    private float lastAbilityQ = -Mathf.Infinity;
    private float lastAbilityE = -Mathf.Infinity;

    // Cached animator param names (avoids errors when params don't exist)
    private System.Collections.Generic.HashSet<string> animParams;



    // --------Movement Properties-----------
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

    public bool IsIdleEast
    {
        get { return isIdleEast; }
        set
        {
            isIdleEast = value;
            if (HasAnimParam("isIdleEast")) animator.SetBool("isIdleEast", isIdleEast);
        }
    }
    public bool IsIdleWest
    {
        get { return isIdleWest; }
        set
        {
            isIdleWest = value;
            if (HasAnimParam("isIdleWest")) animator.SetBool("isIdleWest", isIdleWest);
        }
    }
    public bool IsIdleNorth
    {
        get { return isIdleNorth; }
        set
        {
            isIdleNorth = value;
            if (HasAnimParam("isIdleNorth")) animator.SetBool("isIdleNorth", isIdleNorth);
        }
    }
    public bool IsIdleSouth
    {
        get { return isIdleSouth; }
        set
        {
            isIdleSouth = value;
            if (HasAnimParam("isIdleSouth")) animator.SetBool("isIdleSouth", isIdleSouth);
        }
    }

    // --------Attack Properties-----------
    public bool IsAttacking
    {
        get { return isAttacking; }
        set
        {
            isAttacking = value;
            if (HasAnimParam("isAttacking")) animator.SetBool("isAttacking", isAttacking);
        }
    }
    public bool IsAttackingEast
    {
        get { return isAttackingEast; }
        set
        {
            isAttackingEast = value;
            if (HasAnimParam("isAttackingEast")) animator.SetBool("isAttackingEast", isAttackingEast);
        }
    }

    public bool IsAttackingWest
    {
        get { return isAttackingWest; }
        set
        {
            isAttackingWest = value;
            if (HasAnimParam("isAttackingWest")) animator.SetBool("isAttackingWest", isAttackingWest);
        }
    }




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

        // scene change logic code
        if (PlayerSpawn.nextSpawn != Vector2.zero)
        {
            Vector2 spawnPos = PlayerSpawn.nextSpawn;
            spawnPos.y += 1f; // lift slightly to avoid overlapping tile colliders
            transform.position = spawnPos;

            // clear spawn so it doesn't apply again
            PlayerSpawn.nextSpawn = Vector2.zero;
        }
    }

    bool HasAnimParam(string name) => animator != null && animParams != null && animParams.Contains(name);

    void Update()
    {

        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            movement = Vector2.zero;
            if (rb != null) rb.linearVelocity = Vector2.zero;

            IsRunning = false;
            IsRunningNorth = false;
            IsRunningSouth = false;
            IsRunningEast = false;
            IsRunningWest = false;

            return;
        }

        if (!isAttacking)
        {
            Debug.Log("Player Update running");
            // 🔧 TEMP TEST — remove later
            if (Input.GetKeyDown(KeyCode.H))
            {
                TakeDamage(10);
            }
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

            // --- Idle Logic ---
            if (!isRunning && movement == Vector2.zero)
            {
                // Use lastMovement to determine facing direction when standing still
                Vector2 dir = lastMovement;

                bool pureVertical = Mathf.Approximately(dir.x, 0f);
                bool pureHorizontal = Mathf.Approximately(dir.y, 0f);

                // Reset ALL first (critical for instant switching)
                IsIdleEast = false;
                IsIdleWest = false;
                IsIdleNorth = false;
                IsIdleSouth = false;

                // Set the current direction
                if (pureVertical && dir.y > 0)
                {
                    IsIdleNorth = true;
                }
                else if (pureVertical && dir.y < 0)
                {
                    IsIdleSouth = true;
                }
                else if (pureHorizontal && dir.x > 0)
                {
                    IsIdleEast = true;
                }
                else if (pureHorizontal && dir.x < 0)
                {
                    IsIdleWest = true;
                }
            }
            else
            {
                IsIdleEast = false;
                IsIdleWest = false;
                IsIdleNorth = false;
                IsIdleSouth = false;
            }
        }
        else
        {

            rb.linearVelocity = Vector2.zero;
            movement = Vector2.zero;
            IsRunning = false;
            IsRunningNorth = false;
            IsRunningSouth = false;
            IsRunningEast = false;
            IsRunningWest = false;

        }


        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time >= lastDash + dashCooldown)
        {
            StartDash();
        }

        // --- Ability placeholders ---
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q pressed");
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
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDashing)
        {
            rb.linearVelocity = movement * dashSpeed;
        }
        else if (isAttacking)
        {
            movement = Vector2.zero;
            IsRunning = false;
            IsRunningNorth = false;
            IsRunningSouth = false;
            IsRunningEast = false;
            IsRunningWest = false;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            if (movement != Vector2.zero)
            {
                rb.linearVelocity = movement * runSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
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
        // Set attacking state and stop movement immediately
        IsAttacking = true;
        movement = Vector2.zero;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Get mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Keep on same plane as player

        // Calculate direction from player to mouse
        Vector2 attackDirection = (mouseWorldPos - transform.position).normalized;

        // Reset all attack directions
        IsAttackingEast = false;
        IsAttackingWest = false;

        // Determine attack animation based on mouse direction (horizontal only)
        if (attackDirection.x > 0)
        {

            IsAttackingEast = true;
        }
        // Mouse is to the right
        else
        {

            IsAttackingWest = true;
        } // Mouse is to the left

        // Optional: Update lastMovement so character faces mouse direction
        lastMovement = attackDirection;

        // Detect enemies in range (you might want to adjust attackPoint position based on mouse direction)
        if (attackPoint != null)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Hit " + enemy.name);
            }
        }

        // Reset attack after attack duration (match this to your attack animation length)
        Invoke(nameof(ResetAttack), attackDuration);
    }
    void ResetAttack()
    {
        IsAttacking = false;
        IsAttackingEast = false;
        IsAttackingWest = false;
    }

    // --- Ability stubs (with cooldown so HUD can show it) ---
    public void UseAbilityQ()
    {
        if (Time.time < lastAbilityQ + abilityQCooldown) return;

        lastAbilityQ = Time.time;
        Debug.Log("Ability Q triggered");

        if (abilityQ != null)
        {
            abilityQ.Do();
        }
    }

    public void UseAbilityE()
    {
        if (Time.time < lastAbilityE + abilityECooldown) return;

        lastAbilityE = Time.time;
        Debug.Log("Ability E triggered");

        if (abilityE != null)
        {
            abilityE.Do();
        }
    }

    // --- Animation Updates ---
    void UpdateAnimations()
    {
        if (animator == null) return;


        if (HasAnimParam("isRunning"))
            animator.SetBool("isRunning", isRunning);

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

    public void TryEquipAbility(AbilityCard newAbility)
    {
        if (abilityQ == null)
        {
            abilityQ = newAbility.abilityPrefab;
            return;
        }

        else if (abilityE == null)
        {
            abilityE = newAbility.abilityPrefab;
            return;
        }
        else
        {
            return; //need to implement replacement thing
        }
        
    }

    public void ReplaceAbilitySlot(bool replaceQ, AbilityCard newAbility)
    {
        if (replaceQ)
        {
            abilityQ = newAbility.abilityPrefab;
        }
        else
        {
            abilityE = newAbility.abilityPrefab;
        }
    }

    public void AddBuff(string buffID)
    {
        if (activeBuffs.Contains(buffID))
        {
            return;
        }

        activeBuffs.Add(buffID);
    }
}

