using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeMemory : MonoBehaviour
{
    private class State
    {
        public float originalVolume;
        public float lastTime;
        public bool wasPausedByUs;
        public Coroutine running;
    }

    private readonly Dictionary<AudioSource, State> states = new();

    private State GetState(AudioSource src)
    {
        if (!states.TryGetValue(src, out var st))
        {
            st = new State { originalVolume = src.volume, lastTime = 0f, wasPausedByUs = false, running = null };
            states[src] = st;
        }
        return st;
    }

    public void FadeOutAndPause(AudioSource src, float duration)
    {
        if (src == null) return;

        var st = GetState(src);

        if (st.running != null) StopCoroutine(st.running);
        st.running = StartCoroutine(FadeOutPauseRoutine(src, st, duration));
    }

    public void FadeInAndResume(AudioSource src, float duration)
    {
        if (src == null) return;

        var st = GetState(src);

        if (st.running != null) StopCoroutine(st.running);
        st.running = StartCoroutine(FadeInResumeRoutine(src, st, duration));
    }

    // For your "multiple AudioSources on the same object" case
    public void FadeOutAndPauseAllOnObject(AudioSource any, float duration)
    {
        if (any == null) return;
        foreach (var s in any.GetComponents<AudioSource>())
            FadeOutAndPause(s, duration);
    }

    public void FadeInAndResumeAllOnObject(AudioSource any, float duration)
    {
        if (any == null) return;
        foreach (var s in any.GetComponents<AudioSource>())
            FadeInAndResume(s, duration);
    }

    private IEnumerator FadeOutPauseRoutine(AudioSource src, State st, float duration)
    {
        st.originalVolume = Mathf.Max(st.originalVolume, src.volume); // keep a sane "return" volume
        float startVol = src.volume;

        // if it isn't playing, just ensure it's paused at 0 volume
        if (!src.isPlaying && src.time <= 0f)
        {
            src.volume = 0f;
            src.Pause();
            st.wasPausedByUs = true;
            yield break;
        }

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            src.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }

        src.volume = 0f;
        st.lastTime = src.time;
        src.Pause();                 // <-- timestamp preserved
        st.wasPausedByUs = true;
    }

    private IEnumerator FadeInResumeRoutine(AudioSource src, State st, float duration)
    {
        // resume from lastTime if we paused it before
        if (st.wasPausedByUs)
        {
            // if something stopped it and time reset, restore
            if (src.time <= 0f && st.lastTime > 0f && src.clip != null)
                src.time = Mathf.Clamp(st.lastTime, 0f, src.clip.length - 0.01f);

            src.UnPause();           // <-- continues from paused timestamp
            st.wasPausedByUs = false;
        }
        else
        {
            // if never started, start it
            if (!src.isPlaying)
                src.Play();
        }

        float target = st.originalVolume;
        float startVol = src.volume;
        if (startVol <= 0f) startVol = 0f;

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            src.volume = Mathf.Lerp(startVol, target, t / duration);
            yield return null;
        }

        src.volume = target;
    }
}
