using UnityEngine;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup from;
    [SerializeField] private CanvasGroup to;

    public void OnRegisterClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;

        StartCoroutine(BackendManager.Instance.SignUp(email, password, username,
        session =>
        {
            if (session != null) 
            {
                fader.StartFade(from, to);
            }
        }
        ));
    }
}
