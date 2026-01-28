using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public float fadeDuration = 5f;
    private CanvasGroup next;
    bool running;



    // THIS is what will appear in Button OnClick:
    public void StartFade(CanvasGroup from, CanvasGroup to)
    {
        if (running) return;
        StartCoroutine(FadeSequence(from, to));
    }

    IEnumerator FadeSequence(CanvasGroup from, CanvasGroup to)
    {
        running = true;

        to.alpha = 0f;
        to.gameObject.SetActive(true);

        yield return FadeCanvas(from, 1f, 0f);
        from.gameObject.SetActive(false);
        yield return FadeCanvas(to, 0f, 1f);

        running = false;
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float a, float b)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(a, b, t / fadeDuration);
            yield return null;
        }
        cg.alpha = b;
    }
}
