using UnityEngine;

public class SceneManagerGameplay : MonoBehaviour
{
    [SerializeField] private RewardEngine rewardEngine;
    [SerializeField] private int gemsPerKill = 3;

    private int totalKills = 0;
    private int currentFloor = 1;

    public void RegisterKill()
    {
        totalKills++;

        GemManager.Instance.AddGems(gemsPerKill);

        Debug.Log("Kill registered: " + totalKills);
    }

    public void RoomCleared()
    {
        int reward = rewardEngine.Calculate(currentFloor, totalKills);

        GemManager.Instance.AddGems(reward);

        Debug.Log("Room cleared. Gems awarded: " + reward);

        totalKills = 0; // reset for next room
    }
}