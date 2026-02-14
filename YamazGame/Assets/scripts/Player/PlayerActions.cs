using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;

    [Header("Attack Combo")]
    [SerializeField] private float comboWindow = 0.5f;         // Strict 0.5s window to combo
    [SerializeField] private float comboEnableDelay = 0.2f;    // When can you combo?

    private int comboStep = 0;
    private float lastAttackTime = -999f;
    private bool canCombo = false;

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
            Debug.Log("Combo enabled!");
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
            Debug.Log("Combo expired! Starting fresh.");
            ResetCombo();
        }

        // Only attack if we're ready for the next combo step
        if (!canCombo && comboStep > 0)
        {
            Debug.Log("Can't combo yet, wait for animation");
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
        Debug.Log($"Attack {comboStep} at time {Time.time}");

        // Can't combo again until delay passes
        canCombo = false;
    }

    public void ResetCombo()
    {
        comboStep = 0;
        canCombo = false;
        anim?.ResetAttackStep();
        Debug.Log("Combo reset");
    }
}

