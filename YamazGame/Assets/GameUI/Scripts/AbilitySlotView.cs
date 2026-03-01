using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotView : MonoBehaviour {
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private Image iconImage;          // the Image that displays the ability icon



    // 0 = on cooldown, 1 = ready (HUDController passes player's cooldown value)
    public void SetCooldownNormalized(float normalized){
        if (cooldownFillImage == null) return;
        // overlay full when on cd, empty when ready
        float fill = 1f - normalized;
        cooldownFillImage.fillAmount = Mathf.Clamp01(fill);
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (iconImage == null) return;
        iconImage.sprite = iconSprite;
        iconImage.enabled = (iconSprite != null);
    }
}