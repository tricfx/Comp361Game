using System.Collections;
using UnityEngine;

public class AudioFader : MonoBehaviour
{
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio == null)
        {
            Debug.Log("AudioFader: no AudioSource found");
            yield break;
        }

        Debug.Log($"AudioFader: fading '{gameObject.name}', clip = {(audio.clip != null ? audio.clip.name : "null")}");

        float startVolume = audio.volume;
        Debug.Log($"AudioFader: start volume = {startVolume}");

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audio.volume = 0f;
        audio.Stop();

        Debug.Log($"AudioFader: end volume = {audio.volume}, isPlaying = {audio.isPlaying}");
    }
}