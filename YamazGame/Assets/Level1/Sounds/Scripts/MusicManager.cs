using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public Transform sherma;
    public Transform player;
    public AudioSource overworldMusic;
    public float radius = 5f;
    public float minVolume = 0f;

    private float normalVolume;

    private void Start()
    {
        normalVolume = overworldMusic.volume;
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;

        float dist = Vector2.Distance(player.position, sherma.position);
        float t = 1f - Mathf.Clamp01(dist / radius);
        overworldMusic.volume = Mathf.Lerp(normalVolume, minVolume, t);
    }
}