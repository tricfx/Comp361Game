using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopLogic : MonoBehaviour
{
    [Serializable]
    public class Item
    {
        [Header("Unique ID (e.g. buff1)")]
        public string id;

        [Header("Cost")]
        public int cost = 50;

        [Header("UI")]
        public Button buyButton;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI buttonLabel;

        [Header("Optional effect to enable on purchase")]
        public MonoBehaviour enableComponent;
        public GameObject enableObject;
    }

    [Header("References")]
    [SerializeField] private PlayerGems playerGems;
    [SerializeField] private TextMeshProUGUI gemCountText;

    [Header("Rules")]
    [SerializeField] private int maxAbilitiesTotal = 2;

    [Header("Items")]
    [SerializeField] private Item[] items;

    private int totalPurchased = 0;

    private void Awake()
    {
        if (playerGems == null)
            playerGems = FindFirstObjectByType<PlayerGems>();

        foreach (var item in items)
        {
            if (item == null || item.buyButton == null) continue;

            string capturedId = item.id;
            item.buyButton.onClick.RemoveAllListeners();
            item.buyButton.onClick.AddListener(() => TryBuy(capturedId));
        }
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void TryBuy(string id)
    {
        Item item = FindItem(id);
        if (item == null || playerGems == null) return;

        if (IsPurchased(item)) { RefreshUI(); return; }
        if (totalPurchased >= maxAbilitiesTotal) { RefreshUI(); return; }

        if (!playerGems.SpendGems(item.cost))
        {
            RefreshUI();
            return;
        }

        SetPurchased(item.id);
        totalPurchased++;

        if (item.enableComponent != null) item.enableComponent.enabled = true;
        if (item.enableObject != null) item.enableObject.SetActive(true);

        RefreshUI();
    }


    private void RefreshUI()
    {
        if (gemCountText != null && playerGems != null)
            gemCountText.text = playerGems.GetGems().ToString();

        bool capReached = totalPurchased >= maxAbilitiesTotal;

        foreach (var item in items)
        {
            if (item == null) continue;

            if (item.costText != null)
                item.costText.text = item.cost.ToString();

            bool purchased = IsPurchased(item);
            bool canAfford = playerGems != null && playerGems.GetGems() >= item.cost;

            if (item.buyButton != null)
                item.buyButton.interactable = !purchased && !capReached && canAfford;

            if (item.buttonLabel != null)
            {
                if (purchased) item.buttonLabel.text = "Purchased";
                else if (capReached) item.buttonLabel.text = "Max";
                else if (!canAfford) item.buttonLabel.text = "Need gems";
                else item.buttonLabel.text = "Buy";
            }
        }
    }

    private Item FindItem(string id)
    {
        foreach (var item in items)
            if (item != null && item.id == id)
                return item;
        return null;
    }
    private bool IsPurchased(Item item)
    {
        return itemPurchasedFlags.TryGetValue(item.id, out bool v) && v;
    }

    private readonly System.Collections.Generic.Dictionary<string, bool> itemPurchasedFlags
        = new System.Collections.Generic.Dictionary<string, bool>();

    private void SetPurchased(string id)
    {
        itemPurchasedFlags[id] = true;
    }
}
