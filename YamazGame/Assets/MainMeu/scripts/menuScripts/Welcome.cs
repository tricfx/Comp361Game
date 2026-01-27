using UnityEngine;

public class Welcome : MonoBehaviour
{
    public CanvasGroup welcome;
    public CanvasGroup next;
    public Fader Fader;
    public AudioSource sfx;
    private void Update()
    {
        if (Input.anyKey)
        {
            sfx.Play();
            Fader.StartFade(welcome,next);
        }
    }
    // Update is called once per frame

}
