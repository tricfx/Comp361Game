using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class SupabaseHttp
{
    public static IEnumerator SendRequest(
        string url,
        string method,
        string jsonBody,
        System.Action<string> onSuccess,
        System.Action<string> onError,
        string accessToken = null
    )
    {
        var request = new UnityWebRequest(url, method);
        request.downloadHandler = new DownloadHandlerBuffer();

        if (!string.IsNullOrEmpty(jsonBody))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
        }

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(request.downloadHandler.text);
        }
    }
}
