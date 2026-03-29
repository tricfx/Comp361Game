using UnityEngine;

public class Welcome : MonoBehaviour
{
    public CanvasGroup welcome;
    public CanvasGroup next;
    public Fader Fader;
    public AudioSource sfx;
    private bool notfirst;
    [SerializeField] private LeaderboardUI leaderboard;
    private void Update()
    {
        if (Input.anyKey && !notfirst)
        {
            sfx.Play();
            notfirst = true;

            StartCoroutine(BackendManager.Instance.GetBestRuns(
            runs =>
            {
                Fader.StartFade(welcome, next);
                leaderboard.SetEntries(runs);
                
            }
            ));
        }
    }
    // Update is called once per frame

}
