using System.Collections;
using UnityEngine;

public class SlideFader : MonoBehaviour
{
    public float slideDuration = 0.25f;
    public float slideOffset = 900f;

    public Coroutine SlideIn(CanvasGroup cg) => StartCoroutine(SlideRoutine(cg, true));
    public Coroutine SlideOut(CanvasGroup cg) => StartCoroutine(SlideRoutine(cg, false));

    private IEnumerator SlideRoutine(CanvasGroup cg, bool slideIn)
    {
        if (cg == null) yield break;

        var rt = cg.GetComponent<RectTransform>();
        if (rt == null) yield break;

        // ensure active so it can animate
        cg.gameObject.SetActive(true);

        // make it block input only when visible
        cg.blocksRaycasts = slideIn;
        cg.interactable = slideIn;

        // positions (Y-only)
        Vector2 onPos = Vector2.zero;
        Vector2 offPos = new Vector2(0f, slideOffset); // ABOVE screen

        Vector2 from = slideIn ? offPos : onPos;
        Vector2 to = slideIn ? onPos : offPos;

        // start position
        rt.anchoredPosition = from;

        float t = 0f;
        while (t < slideDuration)
        {
            float u = t / slideDuration;
            // smoothstep easing (simple and clean)
            u = u * u * (3f - 2f * u);

            rt.anchoredPosition = Vector2.LerpUnclamped(from, to, u);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.anchoredPosition = to;

        // if sliding out, disable after
        if (!slideIn)
        {
            cg.blocksRaycasts = false;
            cg.interactable = false;
            cg.gameObject.SetActive(false);
        }
    }
}
