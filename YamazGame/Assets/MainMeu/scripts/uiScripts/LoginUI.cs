using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    public void OnSignInClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        StartCoroutine(BackendManager.Instance.SignIn(email, password));
    }
}
