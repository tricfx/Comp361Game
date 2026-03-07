using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour, IDataPersistence
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private CardDatabase cardDatabase;

    [Header("Attack Combo")]
    [SerializeField] private float comboWindow = 0.45f;

    private int comboStep = 0;
    private float lastAttackTime = -999f;
    private bool canCombo = false;
    private bool comboBuffered = false; // Input pressed before canCombo was true

    public bool IsAttacking => comboStep > 0;
    public int ComboStep => comboStep;

    [Header("Ability Cooldowns")]
    [SerializeField] private float abilityQCooldown = 0f;
    [SerializeField] private float abilityECooldown = 0f;

    private float lastDash = -Mathf.Infinity;
    private float lastAbilityQ = -Mathf.Infinity;
    private float lastAbilityE = -Mathf.Infinity;

    [Header("Abilities")]
    public AbilityCard qAbilityCard;
    public AbilityCard eAbilityCard;
    private IAbility abilityQ;
    private IAbility abilityE;
    private GameObject abilityQObject;
    private GameObject abilityEObject;




    private HUDController hud;

    private void Awake()
    {

        if (!input) input = GetComponent<PlayerInputHandler>();
        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
        var hudObj = GameObject.FindWithTag("HUD");
        if (hudObj != null)
            hud = hudObj.GetComponent<HUDController>();
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
        {
            if (comboStep == 0 || canCombo)
                PerformAttack();
            else
                comboBuffered = true; // Buffer the input for when canCombo becomes true
        }

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
            float remaining = controller.dashCooldown - (Time.time - lastDash);
            if (remaining <= 0f) return 1f;
            return 1f - (remaining / controller.dashCooldown);
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
        comboBuffered = false;
        anim?.ResetAttackStep();
    }

    public void AllowNextCombo()
    {
        canCombo = true;

        // If player already pressed attack while waiting, fire it immediately
        if (comboBuffered)
        {
            comboBuffered = false;
            PerformAttack();
        }
    }

    // Equip an ability into the first free slot (Q then E)
    public void TryEquipAbility(AbilityCard card)
    {
        if (abilityQ == null)
        {
            EquipToSlot(card, true);
        }
        else if (abilityE == null)
        {
            EquipToSlot(card, false);
        }
        else
        {
            // Both full → Open replacement UI
            CardUIManager.Instance.OpenReplacementUI(card);
        }
    }

    // Replace a specific slot
    public void ReplaceAbilitySlot(bool replaceQ, AbilityCard card)
    {
        if (replaceQ)
        {
            abilityQ?.Dispose();
            if (abilityQObject) Destroy(abilityQObject);
            EquipToSlot(card, true);
        }
        else
        {
            abilityE?.Dispose();
            if (abilityEObject) Destroy(abilityEObject);
            EquipToSlot(card, false);
        }
    }

    private void EquipToSlot(AbilityCard card, bool toQ)
    {

        GameObject abilityObj = Instantiate(card.abilityPrefab, transform);

        IAbility ability = abilityObj.GetComponent<IAbility>();
        //ability.Initialize(gameObject);

        if (toQ)
        {
            abilityQ?.Dispose();
            if (abilityQObject) Destroy(abilityQObject);
            abilityQCooldown = card.cooldownSeconds;
            qAbilityCard = card;
            abilityQ = ability;
            abilityQObject = abilityObj;


            Debug.Log($"Equipped {card.abilityID} to Q");
        }
        else
        {
            abilityE?.Dispose();
            if (abilityEObject) Destroy(abilityEObject);
            abilityECooldown = card.cooldownSeconds;
            eAbilityCard = card;
            abilityE = ability;
            abilityEObject = abilityObj;

            Debug.Log($"Equipped {card.abilityID} to E");
        }
        hud?.SetSlotIcon(toQ, card.icon);

    }

    private void EquipByIdToSlot(string cardId, bool toQ)
    {
        if (string.IsNullOrEmpty(cardId)) return;

        var card = cardDatabase.GetCardByID(cardId) as AbilityCard;
        if (card == null) return;
        Debug.Log($"Loading ability {cardId} and {card.icon})");
        ReplaceAbilitySlot(toQ, card); // ensures correct slot

    }

    public void SaveData(ref GameData data)
    {
        if (data.abilities == null || data.abilities.Length < 2)
            data.abilities = new string[2];

        // Store the cardIDs (consistent with CardDatabase lookup)
        data.abilities[0] = qAbilityCard != null ? qAbilityCard.cardID : "";
        data.abilities[1] = eAbilityCard != null ? eAbilityCard.cardID : "";
    }

    public void LoadData(GameData data)
    {
        if (data?.abilities == null || data.abilities.Length < 2) return;
        if (cardDatabase == null) return;

        EquipByIdToSlot(data.abilities[0], true);  // Q
        EquipByIdToSlot(data.abilities[1], false); // E
    }
}

