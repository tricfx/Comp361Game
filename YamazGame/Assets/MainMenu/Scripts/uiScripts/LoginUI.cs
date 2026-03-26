using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup from;
    [SerializeField] private CanvasGroup to;
    [SerializeField] private LeaderboardUI leaderboard;

    public void OnSignInClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        StartCoroutine(BackendManager.Instance.SignIn(email, password,
        session =>
        {
            if (session != null)
            {
                StartCoroutine(BackendManager.Instance.GetBestRun(
                bestRun =>
                {
                    leaderboard.SetPlayerBestRun(bestRun);
                    fader.StartFade(from, to);
                }));
            }
        }
        ));

    }
}
