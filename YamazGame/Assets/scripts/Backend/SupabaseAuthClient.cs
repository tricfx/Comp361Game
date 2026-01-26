using System.Collections;
using UnityEngine;
using System.Text.Json;

public class SupabaseAuthClient
{
    private readonly string baseUrl;
    private readonly string anonKey;

    public SupabaseAuthClient(string supabaseUrl, string anonKey)
    {
        this.baseUrl = $"{supabaseUrl}/auth/v1";
        this.anonKey = anonKey;
    }

    public IEnumerator SignUp(
        string email,
        string password,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/token?grant_type=password",
            "POST",
            json,
            response =>
            {
                var session = JsonSerializer.Deserialize<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError,
            anonKey
        );
    }

    public IEnumerator SignIn(
        string email,
        string password,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/token?grant_type=password",
            "POST",
            json,
            response =>
            {
                var session = JsonSerializer.Deserialize<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError,
            anonKey
        );
    }

    public IEnumerator RefreshSession(
        string refreshToken,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        string json = $"{{\"refresh_token\":\"{refreshToken}\"}}";

        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/token?grant_type=refresh_token",
            "POST",
            json,
            response =>
            {
                var session = JsonSerializer.Deserialize<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError,
            anonKey
        );
    }
    public IEnumerator SignOut(
        string accessToken,
        System.Action onSuccess,
        System.Action<string> onError
    )
    {
        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/logout",
            "POST",
            null,
            _ => onSuccess?.Invoke(),
            onError,
            accessToken
        );
    }
}
