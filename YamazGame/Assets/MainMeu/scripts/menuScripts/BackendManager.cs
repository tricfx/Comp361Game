using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;

public class BackendManager : MonoBehaviour
{
    private SupabaseAuthClient AuthClient;
    private SupabaseGameClient GameClient;
    private SupabaseSessionManager SessionManager;
    
    public static BackendManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        AuthClient = new SupabaseAuthClient(SupabaseAPIKeys.url, SupabaseAPIKeys.key);
        GameClient = new SupabaseGameClient(SupabaseAPIKeys.url, SupabaseAPIKeys.key);

        SessionManager = gameObject.AddComponent<SupabaseSessionManager>();
    }
    public IEnumerator SignUp(string email, string password)
    {
        Debug.Log("Signing up...");
        yield return AuthClient.SignUp(email, password,
        session =>
        {
            Debug.Log("SignUp Successful");
            SessionManager.SetSession(session);
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }

    public IEnumerator SignIn(string email, string password)
    {
        Debug.Log("Signing in...");
        yield return AuthClient.SignIn(email, password,
        session =>
        {
            Debug.Log("SignIn Successful");
            SessionManager.SetSession(session);
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }
}