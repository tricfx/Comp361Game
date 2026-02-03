using UnityEngine;
using TMPro;

public class ForgotPassUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;

    public void OnForgotPassClicked()
    {
        string email = emailInput.text;
        StartCoroutine(BackendManager.Instance.ForgotPassword(email));
    }
}