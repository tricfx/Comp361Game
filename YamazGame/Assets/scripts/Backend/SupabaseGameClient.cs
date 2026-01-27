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

    public IEnumerator GetPlayerState(string accessToken, System.Action<PlayerState> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/get_player_state";

        yield return SupabaseHttp.SendRequest(url, "POST", null, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<PlayerState>>("{\"items\":" + response + "}");
                var array = wrapper.items;
                var state = array.Length > 0 ? array[0] : new PlayerState
                {
                    new_scene_number = 0,
                    new_gems_amount = 0,
                    new_abilities = new string[0],
                    new_left_during_combat = false,
                };
                onSuccess?.Invoke(state);
            },
            onError
        );
    }

    public IEnumerator UpdatePlayerState(string accessToken, PlayerState newState, System.Action<PlayerState> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/update_player_state";
        string body = JsonUtility.ToJson(new PlayerState
        {
            new_scene_number = newState.new_scene_number,
            new_gems_amount = newState.new_gems_amount,
            new_abilities = newState.new_abilities,
            new_left_during_combat = newState.new_left_during_combat
        });

        yield return SupabaseHttp.SendRequest(url, "POST", body, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<PlayerState>>("{\"items\":" + response + "}");
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

    public IEnumerator SubmitRun(string accessToken, long timeMs, bool isCompleted, System.Action<PlayerRun> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/submit_run";
        string body = JsonUtility.ToJson(new PlayerRun { run_time = timeMs, iscompleted = isCompleted });

        yield return SupabaseHttp.SendRequest(url, "POST", body, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<PlayerRun>>("{\"items\":" + response + "}");
                var array = wrapper.items;
                onSuccess?.Invoke(array[0]);
            },
            onError
        );
    }

    public IEnumerator GetBestRuns(string accessToken, System.Action<BestRun[]> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl.Replace("/rpc", "")}/bestcompletiontime?select=username,time&order=time.asc&limit=10";

        yield return SupabaseHttp.SendRequest(url, "GET", null, apikey, accessToken,
            response =>
            {
                var wrapper = JsonUtility.FromJson<Wrapper<BestRun>>("{\"items\":" + response + "}");
                var array = wrapper.items;
                for (int i = 0; i < array.Length; i++)
                    array[i].rank = i + 1;
                onSuccess?.Invoke(array);
            },
            onError
        );
    }
}
