using System.Collections;
using UnityEngine;

public class SupabaseAuthClient
{
    private readonly string baseUrl;
    private readonly string apikey;

    public SupabaseAuthClient(string supabaseUrl, string apikey)
    {
        this.baseUrl = $"{supabaseUrl}/auth/v1";
        this.apikey = apikey;
    }

    public IEnumerator SignUp(
        string email,
        string password,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        string url = $"{baseUrl}/signup";

        var bodyObj = new request
        {
            email = email,
            password = password
        };

        string body = JsonUtility.ToJson(bodyObj);
        

        yield return SupabaseHttp.SendRequest(
            url,
            "POST",
            body,
            apikey,
            null,
            response =>
            {
                var session = JsonUtility.FromJson<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError
        );
    }

    public IEnumerator SignIn(
        string email,
        string password,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        string url = $"{baseUrl}/token?grant_type=password";

        var bodyObj = new request
        {
            email = email,
            password = password
        };

        string body = JsonUtility.ToJson(bodyObj);

        yield return SupabaseHttp.SendRequest(
            url,
            "POST",
            body,
            apikey,
            null,
            response =>
            {
                var session = JsonUtility.FromJson<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError
        );
    }

    public IEnumerator RefreshSession(
        string refreshToken,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        var bodyObj = new RefreshRequest
        {
            refresh_token = refreshToken
        };

        string body = JsonUtility.ToJson(bodyObj);

        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/token?grant_type=refresh_token",
            "POST",
            body,
            apikey,
            null,
            response =>
            {
                var session = JsonUtility.FromJson<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError
        );
    }
    // This signs them in, so its best we direct them to a update password menu
    public IEnumerator ForgotPassword(
        string email,
        System.Action<AuthSession> onSuccess,
        System.Action<string> onError
    )
    {
        var bodyObj = new forgotPasswordRequest {
            email = email
        };

        string body = JsonUtility.ToJson(bodyObj);

        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/recover",
            "POST",
            body,
            apikey,
            null,
            response => {
                var session = JsonUtility.FromJson<AuthSession>(response);
                onSuccess?.Invoke(session);
            },
            onError

        );
    }

    public IEnumerator ChangePassword(
        string accessToken,
        string password,
        System.Action onSuccess,
        System.Action<string> onError
    )
    {
        var bodyObj = new changePasswordRequest {
            password = password
        };

        string body = JsonUtility.ToJson(bodyObj);

        yield return SupabaseHttp.SendRequest(
            $"{baseUrl}/user",
            "PUT",
            body,
            apikey,
            accessToken,
            _ => {
                onSuccess?.Invoke();
            },
            onError
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
            apikey,
            accessToken,
            _ => onSuccess?.Invoke(),
            onError
        );
    }

    [System.Serializable]
    public class request {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class RefreshRequest
    {
        public string refresh_token;
    }

    [System.Serializable]
    public class forgotPasswordRequest {
        public string email;
    }

    [System.Serializable]
    public class changePasswordRequest {
        public string password;
    }

}
