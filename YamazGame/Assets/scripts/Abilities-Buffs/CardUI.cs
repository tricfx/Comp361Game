using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    private Card card;


    public void Setup(Card newReward)
    {
        card = newReward;
        titleText.text = newReward.cardName;
        descriptionText.text = newReward.cardDescription;
        if (newReward.icon != null)
        {
            iconImage.sprite = newReward.icon;
            iconImage.enabled = true;
        }
        
    }

    public void OnClickCard()
    {
        CardUIManager.Instance.OnRewardSelected(card);
    }

    public void OnReplaceCard()
    {
        if (card is AbilityCard)
        {
            
            if (CardUIManager.Instance.player.GetComponent<PlayerActions>().qAbilityCard == card)
            {
                CardUIManager.Instance.OnReplaceSelected(true);
            }
            else
            {
                 CardUIManager.Instance.OnReplaceSelected(false);
            }
        }
    }
}