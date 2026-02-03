using UnityEngine;


public class HUDController : MonoBehaviour{
    [SerializeField] private Player1 player;
    [SerializeField] private HealthBarView healthBarView;
    [SerializeField] private AbilitySlotView dashAbilityView;

    void Update() {
        if (player == null) return;
        if (healthBarView == null) return;

        healthBarView.SetHealth(player.currentHP, player.maxHP);

        if (dashAbilityView != null){
            dashAbilityView.SetCooldownNormalized(player.DashCooldownNormalized);
        }
    }

}