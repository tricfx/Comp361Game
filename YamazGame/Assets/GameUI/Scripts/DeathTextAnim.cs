using UnityEngine;
using TMPro;
using System.Collections;

public class DeathTextAnim : MonoBehaviour
{
    private RectTransform rect;
    private TextMeshProUGUI text;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        // Initial state
        rect.localScale = Vector3.one * 0.8f;

        Color c = text.color;
        c.a = 0;
        text.color = c;

        float t = 0;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 1.5f;

            // Scale
            rect.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one * 1.05f, t);

            // Fade
            c.a = Mathf.Lerp(0, 1, t);
            text.color = c;

            yield return null;
        }

        rect.localScale = Vector3.one;
    }
}