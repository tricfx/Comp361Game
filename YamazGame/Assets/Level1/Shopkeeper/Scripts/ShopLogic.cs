using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardDatabase rewardDatabase;
    [SerializeField] private ShopCardUI[] shopSlots;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerGems playerGems;
    [SerializeField] private TextMeshProUGUI gemCountText;
    [SerializeField] private AudioSource purchaseAudioSource;
    [SerializeField] private AudioSource cannotPurchaseAudioSource;

    private PlayerBuffs playerBuffs;
    private PlayerActions playerActions;

    private static readonly List<string> sessionCardIDs = new();
    private static readonly HashSet<string> purchasedCardIDs = new();

    public static void ResetSessionShop()
    {
        sessionCardIDs.Clear();
        purchasedCardIDs.Clear();
    }

    private void Awake()
    {
        CacheReferences();
        HideAllUnavailablePopups();
        //if (sessionCardIDs.Count == 0) GenerateSessionCards();
        //BuildUI();
    }

    private void OnEnable()
    {
        CacheReferences();
        HideAllUnavailablePopups();
        RefreshUI();
    }

    private void CacheReferences()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        playerBuffs = player.GetComponent<PlayerBuffs>();
        playerActions = player.GetComponent<PlayerActions>();

        if (playerGems == null)
            playerGems = FindFirstObjectByType<PlayerGems>();
    }

    private void GenerateSessionCards()
    {
        sessionCardIDs.Clear();

        List<Card> selected = new();
        int rolls = 0;

        while (selected.Count < 3 && rolls < 300)
        {
            Card reward = rewardDatabase.GetRandomReward();

            if (!IsValidForShopSelection(reward, selected))
            {
                rolls++;
                continue;
            }

            selected.Add(reward);
            rolls++;
        }

        if (selected.Count < 3)
            Debug.LogWarning("less than 3 valid cards found");

        for (int i = 0; i < selected.Count; i++)
            sessionCardIDs.Add(selected[i].cardID);
    }

    private bool IsValidForShopSelection(Card reward, List<Card> selected)
    {
        if (reward == null)
            return false;

        if (selected.Contains(reward))
            return false;

        if (!string.IsNullOrEmpty(reward.dependency))
        {
            if (playerBuffs == null || !playerBuffs.HasBuff(reward.dependency))
                return false;
        }

        if (playerActions != null)
        {
            if (playerActions.qAbilityCard != null && playerActions.qAbilityCard.cardID == reward.cardID)
                return false;

            if (playerActions.eAbilityCard != null && playerActions.eAbilityCard.cardID == reward.cardID)
                return false;
        }

        if (playerBuffs != null && playerBuffs.HasBuff(reward.cardID))
            return false;

        return true;
    }

    private void BuildUI()
    {
        if (shopSlots == null || shopSlots.Length == 0)
            return;

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == null)
                continue;

            if (i >= sessionCardIDs.Count)
            {
                shopSlots[i].gameObject.SetActive(false);
                continue;
            }

            Card card = rewardDatabase.GetCardByID(sessionCardIDs[i]);

            if (card == null)
            {
                shopSlots[i].gameObject.SetActive(false);
                continue;
            }

            shopSlots[i].gameObject.SetActive(true);
            shopSlots[i].Setup(this, card);
        }

        RefreshUI();
    }

    public void TryBuy(Card card, ShopCardUI clickedSlot)
    {
        CacheReferences();

        if (card == null || player == null || playerGems == null)
            return;

        bool alreadyPurchased = purchasedCardIDs.Contains(card.cardID) || AlreadyOwned(card);

        if (alreadyPurchased)
        {
            cannotPurchaseAudioSource.Play();

            clickedSlot.ShowUnavailablePopup("Already purchased!");

            RefreshUI();
            return;
        }

        if (card is AbilityCard)
        {
            bool slotsFull =
                playerActions != null &&
                playerActions.qAbilityCard != null &&
                playerActions.eAbilityCard != null;

            if (slotsFull)
            {
                cannotPurchaseAudioSource.Play();
                clickedSlot.ShowUnavailablePopup("Ability slots are full!");

                RefreshUI();
                return;
            }
        }

        if (playerGems.CurrentGems < card.shopCost)
        {
            cannotPurchaseAudioSource.Play();
            clickedSlot.ShowUnavailablePopup("Not enough gems!");
            RefreshUI();
            return;
        }

        if (!playerGems.TrySpendGems(card.shopCost))
        {
            cannotPurchaseAudioSource.Play();
            clickedSlot.ShowUnavailablePopup("Not enough gems!");
            RefreshUI();
            return;
        }

        if (card is AbilityCard abilityCard)
        {
            playerActions.TryEquipAbility(abilityCard);
        }
        else
        {
            card.Apply(player);
            purchaseAudioSource.Play();
        }

        purchasedCardIDs.Add(card.cardID);
        purchaseAudioSource.Play();

        RefreshUI();
    }

    private bool AlreadyOwned(Card card)
    {
        if (card == null)
            return false;

        if (card is AbilityCard abilityCard)
        {
            if (playerActions == null)
                return false;

            bool inQ = playerActions.qAbilityCard != null &&
                       playerActions.qAbilityCard.cardID == abilityCard.cardID;

            bool inE = playerActions.eAbilityCard != null &&
                       playerActions.eAbilityCard.cardID == abilityCard.cardID;

            return inQ || inE;
        }

        return playerBuffs != null && playerBuffs.HasBuff(card.cardID);
    }

    private void RefreshUI()
    {
        if (gemCountText != null && playerGems != null)
            gemCountText.text = playerGems.CurrentGems.ToString();

        if (shopSlots == null)
            return;

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == null || shopSlots[i].Card == null)
                continue;

            Card card = shopSlots[i].Card;

            bool purchased = purchasedCardIDs.Contains(card.cardID);
            bool owned = AlreadyOwned(card);
            bool canAfford = playerGems != null && playerGems.CurrentGems >= card.shopCost;

            bool canBuy = !purchased && !owned && canAfford;

            //string label;
            //if (purchased) label = "Purchased";
            //else if (owned) label = "Owned";
            //else if (!canAfford) label = "Need gems";
            //else label = "Buy";

            bool greyOut = purchased || owned;

            shopSlots[i].RefreshState(canBuy, greyOut);
        }
    }

    private void HideAllUnavailablePopups()
    {
        if (shopSlots == null) return;

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] != null)
                shopSlots[i].HideUnavailablePopup();
        }
    }
    public void PrepareShop()
    {
        CacheReferences();
        HideAllUnavailablePopups();

        if (sessionCardIDs.Count == 0) GenerateSessionCards();
        BuildUI();
    }
}