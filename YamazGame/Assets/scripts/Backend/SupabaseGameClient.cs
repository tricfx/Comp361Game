using System.Collections;
using UnityEngine;

public class SupabaseGameClient
{
    private readonly string baseUrl;
    private readonly string apikey;
    public SupabaseGameClient(string supabaseUrl, string apikey)
    {
        baseUrl = $"{supabaseUrl}/rest/v1/rpc";
        this.apikey = apikey;
    }

    public IEnumerator GetPlayerState(string accessToken, System.Action<PlayerStateResponse> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/get_player_state";

        yield return SupabaseHttp.SendRequest(url, "POST", null, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<PlayerStateResponse>>("{\"items\":" + response + "}");
                var array = wrapper.items;
                var state = array.Length > 0 ? array[0] : new PlayerStateResponse
                {
                    scene_number = 0,
                    gems_amount = 0,
                    abilities = new string[0],
                    left_during_combat = false,
                };
                onSuccess?.Invoke(state);
            },
            onError
        );
    }

    public IEnumerator UpdatePlayerState(string accessToken, PlayerStateRequest newState, System.Action<PlayerStateResponse> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/update_player_state";
        string body = JsonUtility.ToJson(newState);
        yield return SupabaseHttp.SendRequest(url, "POST", body, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<PlayerStateResponse>>("{\"items\":" + response + "}");
                var state = wrapper.items.Length > 0 ? wrapper.items[0] : null;
                if (state != null) {
                    onSuccess?.Invoke(state);
                }
                else {
                     onError?.Invoke("No player state returned");
                }
            },
            onError
        );
    }

    public IEnumerator SubmitRun(string accessToken, long timeMs, bool isCompleted, System.Action<PlayerRunResponse> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/submit_run";
        string body = JsonUtility.ToJson(new PlayerRunRequest { new_run_time = timeMs, iscompleted = isCompleted });

        yield return SupabaseHttp.SendRequest(url, "POST", body, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<PlayerRunResponse>>("{\"items\":" + response + "}");
                var array = wrapper.items;
                onSuccess?.Invoke(array[0]);
            },
            onError
        );
    }

    public IEnumerator GetBestRuns(string accessToken, System.Action<BestRunResponse[]> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl.Replace("/rpc", "")}/bestcompletiontime?select=username,best_time&order=best_time.asc&limit=10";

        yield return SupabaseHttp.SendRequest(url, "GET", null, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<BestRunResponse>>("{\"items\":" + response + "}");
                var array = wrapper.items;
                for (int i = 0; i < array.Length; i++)
                    array[i].rank = i + 1;
                onSuccess?.Invoke(array);
            },
            onError
        );
    }

    public IEnumerator CreatePlayer(string accessToken, string username, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/create_player";

        var bodyObj = new PlayerRequest
        {
            new_username = username
        };
        string body = JsonUtility.ToJson(bodyObj);

        yield return SupabaseHttp.SendRequest(
            url,
            "POST",
            body,
            apikey,
            accessToken,
            _ => onSuccess?.Invoke(),
            onError
        );
    }
}
