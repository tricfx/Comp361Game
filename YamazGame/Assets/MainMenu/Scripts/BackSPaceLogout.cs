using UnityEngine;

public class BackSPaceLogout : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup from;
    [SerializeField] private CanvasGroup to;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (BackendManager.Instance.SessionManager.AccessToken != null)
            {
                StartCoroutine(BackendManager.Instance.SignOut(
                () =>
                {
                    fader.StartFade(from, to);
                }
                ));
            }
            else
            {
                fader.StartFade(from, to);
            }
            
        }
    }
}
