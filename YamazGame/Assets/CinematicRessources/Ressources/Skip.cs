using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SkipPromptController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float visibleDuration = 5f;
    [SerializeField] private float fadeDuration = 0.4f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    private bool promptVisible = false;
    private Coroutine fadeRoutine;

    void Awake()
    {
        // Safety: auto-grab CanvasGroup if forgotten
        if (canvasGroup == null && skipText != null)
            canvasGroup = skipText.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (skipText != null)
            skipText.gameObject.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    void Update()
    {
        // Any key shows the prompt
        if (!promptVisible && Input.anyKeyDown)
        {
            ShowPrompt();
        }

        // If visible, listen for skip
        if (promptVisible && Input.GetKeyDown(KeyCode.X))
        {
            LoadNextScene();
        }
    }

    void ShowPrompt()
    {
        promptVisible = true;

        if (skipText != null)
            skipText.gameObject.SetActive(true);

        StartFade(1f);

        // Auto-hide after delay
        Invoke(nameof(HidePrompt), visibleDuration);
    }

    void HidePrompt()
    {
        if (!promptVisible) return;

        promptVisible = false;
        StartFade(0f);
    }

    void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvas(targetAlpha));
    }

    IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (Mathf.Approximately(targetAlpha, 0f) && skipText != null)
            skipText.gameObject.SetActive(false);
    }

    void LoadNextScene()
    {
        CancelInvoke(nameof(HidePrompt));

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
