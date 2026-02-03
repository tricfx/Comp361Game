using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour{
    [SerializeField] private Image fillImage;

    // 0-1 fill from current/max (called by HUDController every frame)
    public void SetHealth(float current, float max){
        if (fillImage == null) return;
        if (max <= 0f) return;

        float normalized = current/max;
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}

