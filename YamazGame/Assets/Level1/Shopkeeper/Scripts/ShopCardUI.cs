using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    public TextMeshProUGUI costText;
    public Button buyButton;
    public CanvasGroup canvasGroup;

    [Header("Unavailable Popup")]
    public GameObject unavailablePopupRoot;
    public TextMeshProUGUI unavailablePopupText;

    private ShopLogic shop;
    private Card card;

    private ColorBlock originalColors;
    private bool colorsCached;

    private Coroutine hidePopupCoroutine;

    public Card Card => card;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (buyButton != null)
        {
            originalColors = buyButton.colors;
            colorsCached = true;
        }

        HideUnavailablePopup();
    }

    public void Setup(ShopLogic owner, Card newCard)
    {
        shop = owner;
        card = newCard;

        if (titleText != null)
            titleText.text = newCard.cardName;

        if (descriptionText != null)
            descriptionText.text = newCard.cardDescription;

        if (costText != null)
            costText.text = newCard.shopCost.ToString();

        if (iconImage != null)
        {
            if (newCard.icon != null)
            {
                iconImage.sprite = newCard.icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        HideUnavailablePopup();
    }

    public void RefreshState(bool canBuy, bool greyOut)
    {
        if (buyButton != null)
            buyButton.interactable = true;

        if (canvasGroup != null)
            canvasGroup.alpha = greyOut ? 0.55f : 1f;

        if (buyButton != null && colorsCached)
        {
            ColorBlock colors = originalColors;

            if (greyOut)
            {
                Color faded = new Color(0.6f, 0.6f, 0.6f, 1f);
                colors.normalColor = faded;
                colors.highlightedColor = faded;
                colors.pressedColor = faded;
                colors.selectedColor = faded;
            }

            buyButton.colors = colors;
        }
    }
    public void ShowUnavailablePopup(string message)
    {
        unavailablePopupText.text = message;
        unavailablePopupRoot.SetActive(true);
        if (hidePopupCoroutine != null)
            StopCoroutine(hidePopupCoroutine);
        hidePopupCoroutine = StartCoroutine(HidePopupAfterDelay());
    }

    public void HideUnavailablePopup()
    {
        if (hidePopupCoroutine != null)
        {
            StopCoroutine(hidePopupCoroutine);
            hidePopupCoroutine = null;
        }
        hidePopupCoroutine = null;
        unavailablePopupRoot.SetActive(false);
    }

    private System.Collections.IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSecondsRealtime(10f);
        unavailablePopupRoot.SetActive(false);
        hidePopupCoroutine = null;
    }

    private void OnBuyClicked()
    {
        if (shop != null && card != null)
            shop.TryBuy(card, this);
    }
}

