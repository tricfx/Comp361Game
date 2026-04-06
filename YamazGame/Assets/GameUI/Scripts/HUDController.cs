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
    [SerializeField] private AbilitySlotView abilityQView;
    [SerializeField] private AbilitySlotView abilityEView;
    [SerializeField] private BloodSplatterView bloodSplatterView;

    [Header("Set Icons for Slots")]
    [SerializeField] private AbilitySlotView qSlot;
    [SerializeField] private AbilitySlotView eSlot;

    public void SetSlotIcon(bool toQ, Sprite icon)
    {

        if (toQ) qSlot.SetIcon(icon);
        else eSlot.SetIcon(icon);
    }

    void Update()
    {
        if (playerHealth == null)
        {
             playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("HUDController: PlayerHealth reference is NULL.");
            return;
        }
        }
         if (playerActions == null)
    {
        playerActions = FindFirstObjectByType<PlayerActions>();
        if (playerActions == null) return;
    }

        // Update health bar and health bar text 
        healthBarView.SetHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        healthTextView.SetHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        bloodSplatterView.SetHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);

        // Ability slots Q, E
        abilityQView.SetCooldownNormalized(playerActions.AbilityQCooldownNormalized);
        abilityEView.SetCooldownNormalized(playerActions.AbilityECooldownNormalized);
    }
}