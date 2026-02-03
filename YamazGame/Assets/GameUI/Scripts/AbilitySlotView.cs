using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotView : MonoBehaviour {
    [SerializeField] private Image cooldownFillImage;

    public void SetCooldownNormalized(float normalized){
        if (cooldownFillImage == null) return;

        float fill = 1f - normalized;
        cooldownFillImage.fillAmount = Mathf.Clamp01(fill);
    }
}