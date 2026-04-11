using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFrameSequencePlayer : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 33.33f;

    [SerializeField] private AudioSource appearSFX;
    [SerializeField] private AudioSource disappearSFX;

    [SerializeField] private float startDelay = 2.0f;
    [SerializeField] private float appearSoundDelay = 1.0f;
    [SerializeField] private int disappearSoundFrame = 76;

    private static bool hasPlayedThisSession = false;

    private void Start()
    {
        targetImage.sprite = null;
        targetImage.enabled = false;

        if (hasPlayedThisSession)
            return;

        hasPlayedThisSession = true;
        StartCoroutine(BeginAfterDelay());
    }

    private IEnumerator BeginAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        targetImage.sprite = null;
        targetImage.enabled = true;
        StartCoroutine(PlayAppearSoundDelayed());

        float delay = 1f / fps;

        for (int i = 0; i < frames.Length; i++)
        {
            targetImage.sprite = frames[i];

            if (i == disappearSoundFrame)
                disappearSFX.Play();

            yield return new WaitForSeconds(delay);
        }

        targetImage.sprite = null;
        targetImage.enabled = false;
    }

    private IEnumerator PlayAppearSoundDelayed()
    {
        yield return new WaitForSeconds(appearSoundDelay);
        appearSFX.Play();
    }
}