using UnityEngine;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup from;
    [SerializeField] private CanvasGroup to;
    [SerializeField] private ErrorUI errorUI;

    public void OnRegisterClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        StartCoroutine(BackendManager.Instance.SignUp(email, password,
        session =>
        {
            if (session != null) 
            {
                fader.StartFade(from, to);
            }
        },
        error =>
        {
            errorUI.ShowError($"{error.msg}", 4f);
        }
        ));
    }
}
