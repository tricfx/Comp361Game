using UnityEngine;
using UnityEngine.UI;

public class BloodSplatterView : MonoBehaviour
{
    [SerializeField] private Image bloodImage;
    [SerializeField] private RectTransform rectTransform;

    [Header("Base blood curve")]
    [SerializeField] private float maxAlpha = 1.2f;
    [SerializeField] private float maxScale = 1.8f;
    [SerializeField] private float perfectFitAtHpPercent = 0.2f;

    [Header("Retract behavior")]
    [SerializeField] private int retractBelowHP = 70;
    [SerializeField] private float retractDelay = 8.5f;
    [SerializeField] private float expandSpeed = 8f;
    [SerializeField] private float retractSpeed = 5.5f;

    private int lastHealth = -1;
    private int currentHealth;
    private int currentMaxHealth;

    private float currentAlpha;
    private float currentScale;

    private float targetAlpha;
    private float targetScale;

    private float lastDamageTime = float.NegativeInfinity;

    private void Start()
    {
        transform.SetAsFirstSibling();

        currentAlpha = 0f;
        currentScale = maxScale;
        int baselineHp = Mathf.Clamp(retractBelowHP, 0, currentMaxHealth);
        GetVisualForHealth(baselineHp, currentMaxHealth, out targetAlpha, out targetScale);

        ApplyVisuals(currentAlpha, currentScale);
    }

    private void Update()
    {
        if (currentMaxHealth <= 0)
            return;

        UpdateTargetVisual();
        AnimateTowardsTarget();
    }

    public void SetHealth(int current, int max)
    {
        if (max <= 0)
            return;

        currentHealth = Mathf.Clamp(current, 0, max);
        currentMaxHealth = max;

        GetVisualForHealth(currentHealth, currentMaxHealth, out float hpAlpha, out float hpScale);

        bool firstCall = lastHealth < 0;
        bool tookDamage = !firstCall && currentHealth < lastHealth;
        bool healed = !firstCall && currentHealth > lastHealth;

        if (firstCall)
        {
            currentAlpha = hpAlpha;
            currentScale = hpScale;
            targetAlpha = hpAlpha;
            targetScale = hpScale;
            ApplyVisuals(currentAlpha, currentScale);

            if (currentHealth <= retractBelowHP)
                lastDamageTime = Time.time;
        }
        else if (tookDamage)
        {
            lastDamageTime = Time.time;

            targetAlpha = hpAlpha;
            targetScale = hpScale;
        }
        else if (healed || currentHealth > retractBelowHP)
        {
            targetAlpha = hpAlpha;
            targetScale = hpScale;
        }

        lastHealth = currentHealth;
    }

    private void UpdateTargetVisual()
    {
        if (currentHealth <= retractBelowHP)
        {
            bool shouldRetract = Time.time - lastDamageTime >= retractDelay;

            if (shouldRetract)
            {
                int baselineHp = Mathf.Clamp(retractBelowHP, 0, currentMaxHealth);
                GetVisualForHealth(baselineHp, currentMaxHealth, out targetAlpha, out targetScale);
            }
            else
            {
                GetVisualForHealth(currentHealth, currentMaxHealth, out targetAlpha, out targetScale);
            }
        }
        else
        {
            GetVisualForHealth(currentHealth, currentMaxHealth, out targetAlpha, out targetScale);
        }
    }

    private void AnimateTowardsTarget()
    {
        float dt = Time.deltaTime;

        float alphaStep = (targetAlpha > currentAlpha ? expandSpeed : retractSpeed) * dt;
        float scaleStep = (targetScale < currentScale ? expandSpeed : retractSpeed) * dt;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaStep);
        currentScale = Mathf.MoveTowards(currentScale, targetScale, scaleStep);

        ApplyVisuals(currentAlpha, currentScale);
    }

    private void ApplyVisuals(float alpha, float scale)
    {
        Color c = bloodImage.color;
        c.a = alpha;
        bloodImage.color = c;

        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }

    private void GetVisualForHealth(int current, int max, out float alphaIntensity, out float scale)
    {
        float hpPercent = (float)current / max;

        if (hpPercent > perfectFitAtHpPercent)
        {
            float t = Mathf.InverseLerp(1f, perfectFitAtHpPercent, hpPercent);

            scale = Mathf.Lerp(maxScale, 1.0f, t);
            alphaIntensity = Mathf.Lerp(0f, 0.9f, t);
        }
        else
        {
            scale = 1.0f;

            float t = Mathf.InverseLerp(perfectFitAtHpPercent, 0f, hpPercent);
            alphaIntensity = Mathf.Lerp(0.9f, maxAlpha, t);
        }
    }
}