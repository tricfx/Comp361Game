using System.Collections;
using System.Text.Json;
using UnityEngine;

public class SupabaseGameClient
{
    private readonly string baseUrl;
    public SupabaseGameClient(string supabaseUrl)
    {
        baseUrl = supabaseUrl;
    }

    public IEnumerator GetPlayerState(string accessToken, System.Action<PlayerState> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/rest/v1/rpc/get_player_state";

        yield return SupabaseHttp.SendRequest(url, "POST", null,
            response =>
            {
                var array = JsonSerializer.Deserialize<PlayerState[]>(response);
                var state = array.Length > 0 ? array[0] : new PlayxerState
                {
                    scene_number = 0,
                    gems_amount = 0,
                    abilities = new string[0],
                    left_during_combat = false,
                    updated_at = System.DateTime.UtcNow.ToString("o")
                };
                onSuccess?.Invoke(state);
            },
            onError,
            accessToken
        );
    }

    public IEnumerator UpdatePlayerState(string accessToken, PlayerState newState, System.Action<PlayerState> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/rest/v1/rpc/update_player_state";
        string body = JsonSerializer.Serialize(new
        {
            new_scene_number = newState.scene_number,
            new_gems_amount = newState.gems_amount,
            new_abilities = newState.abilities,
            new_left_during_combat = newState.left_during_combat
        });

        yield return SupabaseHttp.SendRequest(url, "POST", body,
            response =>
            {
                var array = JsonSerializer.Deserialize<PlayerState[]>(response);
                onSuccess?.Invoke(array[0]);
            },
            onError,
            accessToken
        );
    }

    public IEnumerator SubmitRun(string accessToken, long timeMs, bool completed, System.Action<PlayerRun> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/rest/v1/rpc/submit_run";
        string body = JsonSerializer.Serialize(new { run_time = timeMs, completed });

        yield return SupabaseHttp.SendRequest(url, "POST", body,
            response =>
            {
                var array = JsonSerializer.Deserialize<PlayerRun[]>(response);
                onSuccess?.Invoke(array[0]);
            },
            onError,
            accessToken
        );
    }

    public IEnumerator GetBestRuns(string accessToken, System.Action<BestRun[]> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/rest/v1/bestcompletiontime?select=username,time&order=time.asc&limit=10";

        yield return SupabaseHttp.SendRequest(url, "GET", null,
            response =>
            {
                var array = JsonSerializer.Deserialize<BestRun[]>(response);
                for (int i = 0; i < array.Length; i++)
                    array[i].rank = i + 1;
                onSuccess?.Invoke(array);
            },
            onError,
            accessToken
        );
    }
}
