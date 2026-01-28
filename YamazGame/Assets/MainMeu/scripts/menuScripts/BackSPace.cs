using UnityEngine;

public class BackSPace : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup from;
    [SerializeField] private CanvasGroup to;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            fader.StartFade(from, to);
    }
}
