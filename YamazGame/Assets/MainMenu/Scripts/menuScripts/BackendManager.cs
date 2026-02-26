using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using TMPro;

public class BackendManager : MonoBehaviour
{
    private SupabaseAuthClient AuthClient;
    private SupabaseGameClient GameClient;
    public SupabaseSessionManager SessionManager {get; private set;}
    
    public static BackendManager Instance;

    [SerializeField] private ErrorPopup errorPopup;

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
    public IEnumerator SignUp(string email, string password, string username, Action<AuthSession> onSuccess)
    {
        Debug.Log("Signing up...");

        bool signUpSucceeded = false;
        AuthSession sessionResult = null;

        yield return AuthClient.SignUp(email, password,
            session =>
            {
                Debug.Log("SignUp Successful");

                signUpSucceeded = true;
                sessionResult = session;

                SessionManager.SetSession(session);
                onSuccess?.Invoke(session);
            },
            error =>
            {
                Debug.LogError(error);
                ShowError(error);
            }
        );

        if (!signUpSucceeded)
        {
            Debug.LogWarning("Signup failed, skipping CreatePlayer.");
            yield break;
        }

        yield return GameClient.CreatePlayer(SessionManager.AccessToken, username,
            () =>
            {
                Debug.Log($"Created player '{username}'");
            },
            error =>
            {
                Debug.LogError(error);
                ShowError(error);
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
            ShowError(error);
        }
        );
    }

    public IEnumerator SignOut(Action onSuccess)
    {
        Debug.Log("Signing out...");
        yield return AuthClient.SignOut(SessionManager.AccessToken,
        () =>
        {
            Debug.Log("Signed out successfully");
            SessionManager.ClearSession();
            onSuccess?.Invoke();
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
            ShowError(error);
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
                left_during_combat = response.left_during_combat,
                buffs = response.buffs
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
    [System.Serializable]
    private class SupabaseError
    {
        public int code;
        public string error_code;
        public string msg;
    }

    private void ShowError(string rawError)
    {
        string err = "Something went wrong. Please try again.";

        try
        {
            var e = JsonUtility.FromJson<SupabaseError>(rawError);
            string rawLower = rawError.ToLowerInvariant();

            if (e != null && !string.IsNullOrEmpty(e.error_code))
            {
                switch (e.error_code)
                {
                    case "anonymous_provider_disabled":
                        err = "Please enter your credentials";
                        break;

                    //case "user_already_exists":
                    //    err = "An account with this email already exists";
                    //    break;

                    case "invalid_credentials":
                        err = e.msg;
                        break;

                    case "weak_password":
                        err = "Password not long enough";
                        break;


                    case "validation_failed":
                        //err = "Please enter your email";

                        if (rawLower.Contains("invalid format"))
                        {
                            err = "Please enter a valid email address";
                        }
                        else if (rawLower.Contains("missing email") || rawLower.Contains("recovery"))
                        {
                            err = "Please enter your email address";
                        }
                        else
                        {
                            err = e.msg;
                        }
                            break;

                    default:
                        if (!string.IsNullOrEmpty(e.msg)) err = e.msg;
                        break;
                }
            }
        }
        catch
        {
        }

        if (errorPopup != null)
        {
            errorPopup.Show(err);
        }
    }

}

