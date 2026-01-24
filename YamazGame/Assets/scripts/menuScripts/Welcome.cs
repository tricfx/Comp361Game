using UnityEngine;

public class Welcome : MonoBehaviour
{
    public CanvasGroup welcome;
    public CanvasGroup next;
    public Fader Fader;

    private void Update()
    {
        if (Input.anyKey)
        {
            Fader.StartFade(welcome,next);
        }
    }
    // Update is called once per frame

}
