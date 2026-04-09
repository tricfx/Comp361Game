using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Distance Music")]
    public Transform sherma;
    public Transform player;
    public AudioSource overworldMusic;
    public float radius = 5f;
    public float minVolume = 0f;

    [Header("Heartbeat Speed")]
    public float level1HeartbeatPitch = 1f;
    public float level2HeartbeatPitch = 1.15f;
    public float level3HeartbeatPitch = 1.30f;

    [Header("Low Health Effect")]
    public AudioSource heartbeatSource;
    public CanvasGroup lowHealthHaze;
    public float effectDuration = 2f;
    public float fadeTime = 0.25f;

    [Header("30 HP Effect")]
    public float level1MusicMultiplier = 0.45f;
    public float level1HeartbeatVolume = 0.35f;
    public float level1HazeAlpha = 0.12f;

    [Header("20 HP Effect")]
    public float level2MusicMultiplier = 0.30f;
    public float level2HeartbeatVolume = 0.55f;
    public float level2HazeAlpha = 0.22f;

    [Header("10 HP Effect")]
    public float level3MusicMultiplier = 0.15f;
    public float level3HeartbeatVolume = 0.75f;
    public float level3HazeAlpha = 0.35f;

    private float normalVolume;
    private float baseWorldVolume;
    private float lowHealthMusicMultiplier = 1f;
    private Coroutine lowHealthRoutine;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (overworldMusic != null)
        {
            normalVolume = overworldMusic.volume;
            baseWorldVolume = normalVolume;
            overworldMusic.volume = 1f;
        }

        if (heartbeatSource != null)
        {
            heartbeatSource.playOnAwake = false;
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0f;
            heartbeatSource.pitch = 1f;
        }

        if (lowHealthHaze != null)
        {
            lowHealthHaze.alpha = 0f;
        }
    }

    private void Update()
    {
        if (overworldMusic == null) return;

        if (DialogueManager.Instance == null || !DialogueManager.Instance.isDialogueActive)
        {
            if (player != null && sherma != null)
            {
                float dist = Vector2.Distance(player.position, sherma.position);
                float t = 1f - Mathf.Clamp01(dist / radius);
                baseWorldVolume = Mathf.Lerp(normalVolume, minVolume, t);
            }
            else
            {
                baseWorldVolume = normalVolume;
            }
        }

        overworldMusic.volume = baseWorldVolume * lowHealthMusicMultiplier;
    }

    public void TriggerLowHealthEffect(int currentHealth)
    {
        if (currentHealth <= 0)
        {
            StopLowHealthEffectImmediate();
            return;
        }

        if (currentHealth > 30) return;

        int level = 1;
        if (currentHealth <= 10) level = 3;
        else if (currentHealth <= 20) level = 2;

        if (lowHealthRoutine != null)
            StopCoroutine(lowHealthRoutine);

        lowHealthRoutine = StartCoroutine(LowHealthRoutine(level));
    }

    public void StopLowHealthEffectImmediate()
    {
        if (lowHealthRoutine != null)
        {
            StopCoroutine(lowHealthRoutine);
            lowHealthRoutine = null;
        }

        lowHealthMusicMultiplier = 1f;

        if (heartbeatSource != null)
        {
            heartbeatSource.volume = 0f;
            heartbeatSource.pitch = 1f;
            heartbeatSource.Stop();
        }

        if (lowHealthHaze != null)
        {
            lowHealthHaze.alpha = 0f;
        }
    }

    private IEnumerator LowHealthRoutine(int level)
    {
        float targetMusicMultiplier = level1MusicMultiplier;
        float targetHeartbeatVolume = level1HeartbeatVolume;
        float targetHazeAlpha = level1HazeAlpha;
        float targetHeartbeatPitch = level1HeartbeatPitch;

        if (level == 2)
        {
            targetMusicMultiplier = level2MusicMultiplier;
            targetHeartbeatVolume = level2HeartbeatVolume;
            targetHazeAlpha = level2HazeAlpha;
            targetHeartbeatPitch = level2HeartbeatPitch;
        }
        else if (level == 3)
        {
            targetMusicMultiplier = level3MusicMultiplier;
            targetHeartbeatVolume = level3HeartbeatVolume;
            targetHazeAlpha = level3HazeAlpha;
            targetHeartbeatPitch = level3HeartbeatPitch;
        }

        if (heartbeatSource != null && !heartbeatSource.isPlaying)
            heartbeatSource.Play();

        yield return FadeLowHealth(targetMusicMultiplier, targetHeartbeatVolume, targetHazeAlpha, targetHeartbeatPitch, fadeTime);
        yield return new WaitForSeconds(effectDuration);
        yield return FadeLowHealth(1f, 0f, 0f, 1f, fadeTime);

        if (heartbeatSource != null && heartbeatSource.volume <= 0.01f)
            heartbeatSource.Stop();

        lowHealthRoutine = null;
    }

    private IEnumerator FadeLowHealth(float musicTarget, float heartbeatTarget, float hazeTarget, float pitchTarget, float duration)
    {
        float startMusic = lowHealthMusicMultiplier;
        float startHeartbeat = heartbeatSource != null ? heartbeatSource.volume : 0f;
        float startHaze = lowHealthHaze != null ? lowHealthHaze.alpha : 0f;
        float startPitch = heartbeatSource != null ? heartbeatSource.pitch : 1f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = duration <= 0f ? 1f : time / duration;

            lowHealthMusicMultiplier = Mathf.Lerp(startMusic, musicTarget, t);

            if (heartbeatSource != null)
                heartbeatSource.volume = Mathf.Lerp(startHeartbeat, heartbeatTarget, t);

            if (heartbeatSource != null)
                heartbeatSource.pitch = Mathf.Lerp(startPitch, pitchTarget, t);

            if (lowHealthHaze != null)
                lowHealthHaze.alpha = Mathf.Lerp(startHaze, hazeTarget, t);

            yield return null;
        }

        lowHealthMusicMultiplier = musicTarget;

        if (heartbeatSource != null)
            heartbeatSource.volume = heartbeatTarget;

        if (heartbeatSource != null)
            heartbeatSource.pitch = pitchTarget;

        if (lowHealthHaze != null)
            lowHealthHaze.alpha = hazeTarget;
    }
}