using UnityEngine;

public class fadeButton : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup from;
    [SerializeField] private CanvasGroup to;

    public void Click()
    {
        fader.StartFade(from, to);
    }
}
