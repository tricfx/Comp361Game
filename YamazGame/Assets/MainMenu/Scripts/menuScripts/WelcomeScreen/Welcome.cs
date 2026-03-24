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
            
            StartCoroutine(BackendManager.Instance.GetBestRuns(
            runs =>
            {
                leaderboard.SetEntries(runs);
                Fader.StartFade(welcome, next);
            }
            ));
        }
    }
    // Update is called once per frame

}
