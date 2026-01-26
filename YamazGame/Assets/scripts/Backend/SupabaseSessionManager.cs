using UnityEngine;
using System.Collections;

public class SupabaseSessionManager : MonoBehaviour
{
    public SupabaseAuthClient AuthClient;

    public AuthSession CurrentSession;
    private float expiryTime;

    public void SetSession(AuthSession session)
    {
        CurrentSession = session;
        expiryTime = Time.time + session.expires_in;
    }

    private void Update()
    {
        if (CurrentSession == null) return;

        if (Time.time >= expiryTime - 60)
        {
            StartCoroutine(Refresh());
        }
    }

    private IEnumerator Refresh()
    {
        yield return AuthClient.RefreshSession(
            CurrentSession.refresh_token,
            newSession =>
            {
                Debug.Log("Session refreshed");
                SetSession(newSession);
            },
            error =>
            {
                Debug.LogError("Refresh failed: " + error);
                CurrentSession = null;
            }
        );
    }

    public string AccessToken =>
        CurrentSession?.access_token;
}
