using UnityEngine;

// Drives health bar, ability cooldowns, dialogue hide, and death overlay.
// Assign Player + all 3 ability views. 
public class HUDController : MonoBehaviour
{
    [Header("Player & Views")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private HealthBarView healthBarView;
    [SerializeField] private HealthTextView healthTextView;
    [SerializeField] private AbilitySlotView dashAbilityView;
    [SerializeField] private AbilitySlotView abilityQView;
    [SerializeField] private AbilitySlotView abilityEView;
    [SerializeField] private BloodSplatterView bloodSplatterView;

    [Header("Dialogue & Death")]
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private GameObject deathOverlayPanel;

    private bool deathShown = false;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (deathOverlayPanel != null)
            deathOverlayPanel.SetActive(false);

        deathShown = false;
    }

    void Update()
    {
        if (playerHealth == null)
        {
            Debug.LogError("HUDController: PlayerHealth reference is NULL.");
            return;
        }
        if (playerActions == null) return;

        // health bar - only update when we have the view so no missing ref
        if (healthBarView != null)
            healthBarView.SetHealth(
                playerHealth.CurrentHealth,
                playerHealth.MaxHealth
            );

        if (healthTextView != null)
        {
            healthTextView.SetHealth(
                playerHealth.CurrentHealth,
                playerHealth.MaxHealth
            );
        }
        if (bloodSplatterView != null)
        {
            bloodSplatterView.SetHealth(
                playerHealth.CurrentHealth,
                playerHealth.MaxHealth
            );
        }

        // all three ability slots (dash, Q, E)
        if (playerActions != null)
        {
            if (dashAbilityView != null)
                dashAbilityView.SetCooldownNormalized(playerActions.DashCooldownNormalized);
            if (abilityQView != null)
                abilityQView.SetCooldownNormalized(playerActions.AbilityQCooldownNormalized);
            if (abilityEView != null)
                abilityEView.SetCooldownNormalized(playerActions.AbilityECooldownNormalized);
        }

        // hide HUD when talking to npc so it doesn't sit on top of dialogue
        if (hudCanvasGroup != null && DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.isDialogueActive)
                hudCanvasGroup.alpha = 0f;
            else
                hudCanvasGroup.alpha = 1f;
        }

        // show "you died" panel once when player dies
        if (deathOverlayPanel != null && playerHealth.IsDead && !deathShown)
        {
            deathOverlayPanel.SetActive(true);
            deathShown = true;
        }
    }
}
