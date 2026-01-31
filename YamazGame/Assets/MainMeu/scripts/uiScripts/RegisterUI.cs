using UnityEngine;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    public void OnRegisterClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        StartCoroutine(BackendManager.Instance.SignUp(email, password));
    }
}
