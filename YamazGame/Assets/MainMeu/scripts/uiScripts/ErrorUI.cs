using TMPro;
using UnityEngine;
using System.Collections;

public class ErrorUI : MonoBehaviour
{
    public TextMeshProUGUI errorText;

    void Start()
    {
        errorText.enabled = false;
    }

    public void ShowError(string message, float duration = 3f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowTemporarily(message, duration));
    }

    IEnumerator ShowTemporarily(string message, float duration)
    {
        errorText.text = message;
        errorText.enabled = true;

        yield return new WaitForSeconds(duration);

        errorText.enabled = false;
    }

    public void HideError()
    {
        errorText.gameObject.SetActive(false);
    }

}
