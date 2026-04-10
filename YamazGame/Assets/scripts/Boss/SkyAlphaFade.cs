using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class SkyAlphaFade : MonoBehaviour
{
    [SerializeField] private float fadeDelay = 2f;
    [SerializeField] private float fadeDuration = 3f;

    private RawImage img;
    private Coroutine routine;

    private void Awake()
    {
        img = GetComponent<RawImage>();
        Color c = img.color;
        c.a = 0f;
        img.color = c;
    }

    public void StartFade()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSecondsRealtime(fadeDelay);

        float t = 0f;
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }
}