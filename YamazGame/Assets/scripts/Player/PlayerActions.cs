using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;

    [Header("Attack Combo")]
    [SerializeField] private float comboWindow = 0.45f;

    private int comboStep = 0;
    private float lastAttackTime = -999f;
    private bool canCombo = false;

    public bool IsAttacking => comboStep > 0;

    [Header("Ability Cooldowns")]
    [SerializeField] public float dashCooldown = 1f;
    [SerializeField] private float abilityQCooldown = 3f;
    [SerializeField] private float abilityECooldown = 5f;

    private float lastDash = -Mathf.Infinity;
    private float lastAbilityQ = -Mathf.Infinity;
    private float lastAbilityE = -Mathf.Infinity;

    [Header("Abilities")]
    private IAbility abilityQ;
    private IAbility abilityE;

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInputHandler>();
        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        // Block all actions during dialogue
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
            return;

        if (Input.GetKeyDown(KeyCode.H))
            GetComponent<PlayerHealth>()?.TakeDamage(10);

        if (Input.GetKeyDown(KeyCode.K))
            GetComponent<PlayerHealth>()?.TakeDamage(100);

        if (controller && controller.IsDashing)
            return;

        // Check if combo window expired
        if (comboStep > 0 && Time.time - lastAttackTime > comboWindow)
            ResetCombo();

        if (input.AttackPressed)
            PerformAttack();

        if (input.QPressed && Time.time >= lastAbilityQ + abilityQCooldown)
        {
            lastAbilityQ = Time.time;
            abilityQ?.Do();
        }

        if (input.EPressed && Time.time >= lastAbilityE + abilityECooldown)
        {
            lastAbilityE = Time.time;
            abilityE?.Do();
        }
    }

    public float DashCooldownNormalized
    {
        get
        {
            float remaining = dashCooldown - (Time.time - lastDash);
            if (remaining <= 0f) return 1f;
            return 1f - (remaining / dashCooldown);
        }
    }

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

    private void PerformAttack()
    {
        if (comboStep > 0 && Time.time - lastAttackTime > comboWindow)
            ResetCombo();

        if (!canCombo && comboStep > 0)
            return;

        lastAttackTime = Time.time;
        comboStep++;

        if (comboStep > 3)
            comboStep = 1;

        anim?.TriggerAttackCombo(comboStep);
        canCombo = false;
    }

    public void ResetCombo()
    {
        comboStep = 0;
        canCombo = false;
        anim?.ResetAttackStep();
    }

    public void AllowNextCombo()
    {
        canCombo = true;
    }

    // Equip an ability into the first free slot (Q then E)
    public void TryEquipAbility(AbilityCard card)
    {
        if (abilityQ == null)
        {
            // TODO: instantiate and assign card.abilityPrefab as abilityQ
            Debug.Log($"Equipped {card.abilityID} to Q slot");
            return;
        }
        else if (abilityE == null)
        {
            // TODO: instantiate and assign card.abilityPrefab as abilityE
            Debug.Log($"Equipped {card.abilityID} to E slot");
            return;
        }
        else
        {
            Debug.Log("Both ability slots full — replacement UI needed");
            // TODO: prompt player to replace Q or E
        }
    }

    // Replace a specific slot
    public void ReplaceAbilitySlot(bool replaceQ, AbilityCard card)
    {
        if (replaceQ)
        {
            // TODO: instantiate and assign card.abilityPrefab as abilityQ
            Debug.Log($"Replaced Q slot with {card.abilityID}");
        }
        else
        {
            // TODO: instantiate and assign card.abilityPrefab as abilityE
            Debug.Log($"Replaced E slot with {card.abilityID}");
        }
    }
}

