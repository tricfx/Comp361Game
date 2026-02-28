using UnityEngine;

public class Welcome : MonoBehaviour
{
    public CanvasGroup welcome;
    public CanvasGroup next;
    public Fader Fader;
    public AudioSource sfx;
    [SerializeField] private LeaderboardUI leaderboard;
    private void Update()
    {
        if (Input.anyKey)
        {
            sfx.Play();
            Fader.StartFade(welcome, next);
            StartCoroutine(BackendManager.Instance.GetBestRuns(
            runs =>
            {
                leaderboard.SetEntries(runs);
                
            }
            ));
        }
    }
    // Update is called once per frame

}
