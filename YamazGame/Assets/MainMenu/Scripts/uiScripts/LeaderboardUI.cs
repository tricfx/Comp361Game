using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] rankTexts;
    [SerializeField] private TMP_Text[] nameTexts;
    [SerializeField] private TMP_Text[] timeTexts;
    [SerializeField] private TMP_Text playerBestRunTimeText;
    [SerializeField] private TMP_Text playerBestRunRankText;
    [SerializeField] private TMP_Text playerBestRunUsernameText;

    public void SetEntries(BestRunResponse[] entries)
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < entries.Length)
            {
                rankTexts[i].text = entries[i].rank.ToString();
                nameTexts[i].text = entries[i].username;
                timeTexts[i].text = FormatTime(entries[i].best_time);
            }
            else
            {
                rankTexts[i].text = "-";
                nameTexts[i].text = "---";
                timeTexts[i].text = "--:--:--";
            }
        }
    }

    public void SetPlayerBestRun(BestRunResponse bestRun)
    {
        playerBestRunRankText.text = bestRun.rank > 0 ? bestRun.rank.ToString() : "N/A";
        playerBestRunUsernameText.text = bestRun.username;
        playerBestRunTimeText.text = FormatTime(bestRun.best_time);
    }
        
    private string FormatTime(long time)
    {
        float totalSeconds = time / 1000f;

        // Max allowed time: 99 hours
        if (totalSeconds <= 0 || totalSeconds > 99 * 3600)
            return "N/A";

        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }


}
