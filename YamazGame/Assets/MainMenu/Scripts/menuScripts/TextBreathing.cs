using TMPro;
using UnityEngine;

public class TextBreathing : MonoBehaviour
{
    [Header("Breathing")]
    public float scaleAmount = 0.03f;
    public float speed = 1.2f;
    public float alphaMin = 0.75f;
    public float alphaMax = 1.0f;

    Vector3 baseScale;
    TextMeshProUGUI tmp;
    Color baseColor;

    void Awake()
    {
        baseScale = transform.localScale;
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp != null) baseColor = tmp.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
        float s = 1f + Mathf.Lerp(-scaleAmount, scaleAmount, t);
        transform.localScale = baseScale * s;

        if (tmp != null)
        {
            float a = Mathf.Lerp(alphaMin, alphaMax, t);
            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
        }
    }
}
