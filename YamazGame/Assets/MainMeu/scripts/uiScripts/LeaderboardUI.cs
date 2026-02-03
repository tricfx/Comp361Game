using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] rankTexts;
    [SerializeField] private TMP_Text[] nameTexts;
    [SerializeField] private TMP_Text[] timeTexts;

    public void SetEntries(BestRunResponse[] entries)
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < entries.Length)
            {
                rankTexts[i].text = entries[i].rank.ToString();
                nameTexts[i].text = entries[i].username;
                timeTexts[i].text = entries[i].best_time.ToString();
            }
            else
            {
                rankTexts[i].text = "-";
                nameTexts[i].text = "---";
                timeTexts[i].text = "--:--";
            }
        }
    }
}
