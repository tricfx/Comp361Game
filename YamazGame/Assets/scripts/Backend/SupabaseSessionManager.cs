using UnityEngine;
using System.Collections;

public class SupabaseSessionManager : MonoBehaviour
{
    public SupabaseAuthClient AuthClient;
    public float refreshBufferSeconds = 60f;
    public AuthSession CurrentSession { get; private set; }

    private float expiryTime;
    private bool isRefreshing;

    public void SetSession(AuthSession session)
    {
        if (session == null) return;

        CurrentSession = session;
        expiryTime = Time.realtimeSinceStartup + session.expires_in;
        isRefreshing = false;

        Debug.Log($"Session set. Expires in {session.expires_in} seconds.");
    }

    public string AccessToken => CurrentSession?.access_token;

    private void Update()
    {
        if (CurrentSession == null) return;

        if (!isRefreshing && Time.realtimeSinceStartup >= expiryTime - refreshBufferSeconds)
        {
            StartCoroutine(RefreshCoroutine());
        }
    }

    private IEnumerator RefreshCoroutine()
    {
        if (CurrentSession == null || string.IsNullOrEmpty(CurrentSession.refresh_token))
            yield break;

        isRefreshing = true;

        yield return AuthClient.RefreshSession(
            CurrentSession.refresh_token,
            newSession =>
            {
                if (newSession != null)
                {
                    CurrentSession.access_token = newSession.access_token;
                    CurrentSession.refresh_token = newSession.refresh_token;
                    expiryTime = Time.realtimeSinceStartup + newSession.expires_in;

                    Debug.Log($"Session refreshed successfully. Next refresh in {newSession.expires_in - refreshBufferSeconds} seconds.");
                }
                else
                {
                    Debug.LogError("Refresh failed: received null session");
                    CurrentSession = null;
                }

                isRefreshing = false;
            },
            error =>
            {
                Debug.LogError("Refresh failed: " + error);
                CurrentSession = null;
                isRefreshing = false;
            }
        );
    }
    public void ClearSession()
    {
        CurrentSession = null;
        isRefreshing = false;
        expiryTime = 0;
        Debug.Log("Session cleared.");
    }
}
