using UnityEngine;

public class Player1 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;                // Base movement speed
    public float dashSpeed = 12f;           // Dash speed
    public float dashDuration = 0.2f;       // How long dash lasts
    public float dashCooldown = 1f;         // Cooldown before next dash

    [Header("Player Stats")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Ability Placeholders")]
    public GameObject abilityQ;             // Assign in Inspector
    public GameObject abilityE;             // Assign in Inspector

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isDashing = false;
    private float dashTime = 0f;
    private float lastDash = -Mathf.Infinity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
    }

    void Update()
    {
        // --- Input ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDash + dashCooldown)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = movement * dashSpeed;
        }
        else
        {
            rb.linearVelocity = movement * speed;
        }
    }

    // --- Dash Logic ---
    void StartDash()
    {
        if (movement == Vector2.zero) return; // can't dash without direction
        isDashing = true;
        dashTime = dashDuration;
        lastDash = Time.time;
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
        // Placeholder: animators will hook animation here
        Debug.Log("Attack triggered");
    }

    // --- Ability stubs ---
    void UseAbilityQ()
    {
        Debug.Log("Ability Q triggered");
        // Placeholder for ability logic
    }

    void UseAbilityE()
    {
        Debug.Log("Ability E triggered");
        // Placeholder for ability logic
    }

    // --- Take Damage method ---
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log("Player HP: " + currentHP);
        // Placeholder for death or UI update
    }
}

