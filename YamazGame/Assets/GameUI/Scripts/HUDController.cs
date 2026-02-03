using UnityEngine;

// Drives health bar, ability cooldowns, dialogue hide, and death overlay.
// Assign Player + all 3 ability views. 
public class HUDController : MonoBehaviour
{
    [Header("Player & Views")]
    [SerializeField] private Player1 player;
    [SerializeField] private HealthBarView healthBarView;
    [SerializeField] private AbilitySlotView dashAbilityView;
    [SerializeField] private AbilitySlotView abilityQView;
    [SerializeField] private AbilitySlotView abilityEView;

    [Header("Dialogue & Death")]
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private GameObject deathOverlayPanel;

    void Update()
    {
        if (player == null) return;

        // health bar - only update when we have the view so no missing ref
        if (healthBarView != null)
            healthBarView.SetHealth(player.currentHP, player.maxHP);

        // all three ability slots (dash, Q, E)
        if (dashAbilityView != null)
            dashAbilityView.SetCooldownNormalized(player.DashCooldownNormalized);
        if (abilityQView != null)
            abilityQView.SetCooldownNormalized(player.AbilityQCooldownNormalized);
        if (abilityEView != null)
            abilityEView.SetCooldownNormalized(player.AbilityECooldownNormalized);

        // hide HUD when talking to npc so it doesn't sit on top of dialogue
        if (hudCanvasGroup != null && DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.isDialogueActive)
                hudCanvasGroup.alpha = 0f;
            else
                hudCanvasGroup.alpha = 1f;
        }

        // show "you died" panel when hp hits 0
        if (deathOverlayPanel != null)
            deathOverlayPanel.SetActive(player.currentHP <= 0);
    }
}
