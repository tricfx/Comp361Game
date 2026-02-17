using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;

    [Header("Attack Combo")]
    [SerializeField] private float comboWindow = 0.5f;         // Strict 0.5s window to combo
    [SerializeField] private float comboEnableDelay = 0f;    // When can you combo?

    private int comboStep = 0;
    private float lastAttackTime = -999f;
    private bool canCombo = false;

    public bool IsAttacking => comboStep > 0;

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInputHandler>();
        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (controller && controller.IsDashing)
            return;

        // Auto-enable combo after delay
        if (comboStep > 0 && !canCombo && Time.time - lastAttackTime > comboEnableDelay)
        {
            canCombo = true;

        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<PlayerHealth>()?.TakeDamage(100);
        }

        // Check if combo window expired - RESET if too late
        if (comboStep > 0 && Time.time - lastAttackTime > comboWindow)
        {
            ResetCombo();
        }

        if (input.AttackPressed)
        {
            PerformAttack();
        }

        if (input.QPressed)
        {
            Debug.Log("Ability Q (placeholder)");
        }

        if (input.EPressed)
        {
            Debug.Log("Ability E (placeholder)");
        }
    }

    private void PerformAttack()
    {
        // Check if trying to attack outside combo window
        if (comboStep > 0 && Time.time - lastAttackTime > comboWindow)
        {

            ResetCombo();
        }

        // Only attack if we're ready for the next combo step
        if (!canCombo && comboStep > 0)
        {

            return;
        }

        // Increment combo
        lastAttackTime = Time.time;
        comboStep++;

        if (comboStep > 3)
        {
            comboStep = 1;
        }

        // Trigger the appropriate attack
        anim?.TriggerAttackCombo(comboStep);


        // Can't combo again until delay passes
        canCombo = false;
    }

    public void ResetCombo()
    {
        comboStep = 0;
        canCombo = false;
        anim?.ResetAttackStep();

    }
}

