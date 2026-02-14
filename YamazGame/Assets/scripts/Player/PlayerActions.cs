using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;

    [Header("Attack Combo")]
    [SerializeField] private float comboWindow = 1f; // Time window to continue combo

    private int comboStep = 0;              // Current combo step (0, 1, 2, 3)
    private float lastAttackTime = -999f;   // When last attack happened
    private bool canCombo = false;          // Can we continue the combo?

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInputHandler>();
        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        // Optional: don't allow actions while dashing
        if (controller && controller.IsDashing)
            return;

        // Check if combo window expired
        if (Time.time - lastAttackTime > comboWindow)
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
        // Only attack if we're ready for the next combo step
        if (!canCombo && comboStep > 0)
        {
            Debug.Log("Can't combo yet, wait for animation");
            return;
        }

        lastAttackTime = Time.time;
        comboStep++;

        if (comboStep > 3)
        {
            ResetCombo();
            comboStep = 1;
        }

        // Trigger the appropriate attack
        anim?.TriggerAttackCombo(comboStep);
        Debug.Log($"Attack {comboStep}");

        // Can't combo again until animation allows it
        canCombo = false;
    }

    // Call this from Animation Event in the middle of each attack animation
    public void EnableCombo()
    {
        canCombo = true;
        lastAttackTime = Time.time; // ADD THIS LINE - refreshes the timer!
        Debug.Log("Combo enabled!");
    }

    // Call this to reset combo
    public void ResetCombo()
    {
        comboStep = 0;
        canCombo = false;
        Debug.Log("Combo reset");
    }
}

