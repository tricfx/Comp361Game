using UnityEngine;
using UnityEngine.UI;

public class BloodSplatterView : MonoBehaviour
{
    [SerializeField] private Image bloodImage;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float maxAlpha = 1.2f;
    [SerializeField] private float maxScale = 1.8f;

    void Start()
    {
        // Ensure blood renders BELOW other HUD elements
        transform.SetAsFirstSibling();
    }

    public void SetHealth(int current, int max)
    {
        if (bloodImage == null) return;

        float hpPercent = (float)current / max;

        // Threshold where splatter exactly fits screen
        float thresholdHPPercent = 0.2f; // 20% HP

        float scale;
        float alphaIntensity;

        if (hpPercent > thresholdHPPercent)
        {
            
            // When full hp to 20% hp, blood grows inward until it perfectly fits the screen
            float t = Mathf.InverseLerp(1f, thresholdHPPercent, hpPercent);

            scale = Mathf.Lerp(maxScale, 1.0f, t);

            alphaIntensity = Mathf.Lerp(0f, 0.9f, t);
        }
        else
        {
            
            // When below 20% hp, keeps perfect scale, increases intensity only
            scale = 1.0f;

            float t = Mathf.InverseLerp(thresholdHPPercent, 0f, hpPercent);

            alphaIntensity = Mathf.Lerp(0.9f, maxAlpha, t);
        }

        // Apply alpha
        Color c = bloodImage.color;
        c.a = alphaIntensity;
        bloodImage.color = c;

        // Apply scale
        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }
}