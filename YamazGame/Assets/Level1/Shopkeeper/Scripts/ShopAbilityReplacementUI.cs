using UnityEngine;

public class ShopAbilityReplacementUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private CardUI[] replacementSlots;

    private PlayerActions playerActions;
    private ShopLogic owner;
    private AbilityCard pendingCard;
    private ShopCardUI pendingShopSlot;

    public void Open(ShopLogic shopLogic, PlayerActions actions, AbilityCard newCard, ShopCardUI clickedSlot)
    {
        if (actions == null || newCard == null)
            return;

        owner = shopLogic;
        playerActions = actions;
        pendingCard = newCard;
        pendingShopSlot = clickedSlot;

        replacementSlots[0].Setup(playerActions.qAbilityCard);
        replacementSlots[1].Setup(playerActions.eAbilityCard);

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void ReplaceQ()
    {
        ConfirmReplace(true);
    }

    public void ReplaceE()
    {
        ConfirmReplace(false);
    }

    public void Cancel()
    {
        pendingCard = null;
        pendingShopSlot = null;
        owner = null;
        playerActions = null;

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ConfirmReplace(bool replaceQ)
    {
        if (owner == null || pendingCard == null)
            return;

        owner.CompleteAbilityReplacementPurchase(replaceQ, pendingCard, pendingShopSlot);
        Cancel();
    }
}