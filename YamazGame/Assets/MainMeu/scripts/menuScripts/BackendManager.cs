using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;

public class BackendManager : MonoBehaviour
{
    private SupabaseAuthClient AuthClient;
    private SupabaseGameClient GameClient;
    public SupabaseSessionManager SessionManager {get; private set;}
    
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
    public IEnumerator SignUp(string email, string password, Action<AuthSession> onSuccess)
    {
        Debug.Log("Signing up...");
        yield return AuthClient.SignUp(email, password,
        session =>
        {
            Debug.Log("SignUp Successful");
            SessionManager.SetSession(session);
            onSuccess?.Invoke(session);
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }

    public IEnumerator SignIn(string email, string password, Action<AuthSession> onSuccess)
    {
        Debug.Log("Signing in...");
        yield return AuthClient.SignIn(email, password,
        session =>
        {
            Debug.Log("SignIn Successful");
            SessionManager.SetSession(session);
            onSuccess?.Invoke(session);
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }

    public IEnumerator ForgotPassword(string email)
    {
        Debug.Log("User forgot password");
        yield return AuthClient.ForgotPassword(email,
        session =>
        {
            SessionManager.SetSession(session);
            Debug.Log("User is signed in");
        },
        error =>
        {
            Debug.Log(error);
        }
        );
    }

    public IEnumerator UpdatePlayerState(PlayerStateRequest newState)
    {
        Debug.Log("Updating player state");
        yield return GameClient.UpdatePlayerState(SessionManager.AccessToken, newState,
        _ =>
        {
            Debug.Log("Player state updated successfully");
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }

    public IEnumerator GetPlayerState()
    {
        Debug.Log("Fetching player state");
        yield return GameClient.GetPlayerState(SessionManager.AccessToken,
        response =>
        {
            GameData data = new GameData
            {
                sceneIndex = response.scene_number,
                gemsCollected = response.gems_amount,
                abilities = response.abilities,
                left_during_combat = response.left_during_combat
            };

            DataPersistenceManager.instance.gameData = data;
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }

    public IEnumerator GetBestRuns(Action<BestRunResponse[]> onSuccess)
    {
        Debug.Log("Fetching best runs");
        yield return GameClient.GetBestRuns(SessionManager.AccessToken,
        response =>
        {
            onSuccess?.Invoke(response);
        },
        error =>
        {
            Debug.LogError(error);
        }
        );
    }

}