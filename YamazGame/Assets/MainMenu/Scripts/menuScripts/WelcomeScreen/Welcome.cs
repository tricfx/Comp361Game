using UnityEngine;

public class Welcome : MonoBehaviour
{
    public CanvasGroup welcome;
    public CanvasGroup next;
    public Fader Fader;
    public AudioSource sfx;
    private bool notfirst;
    [SerializeField] private LeaderboardUI leaderboard;

    private void Start()
    {
        CursorManager.Instance.ShowCursor();
         StartCoroutine(BackendManager.Instance.GetBestRuns(
            runs =>
            {
                
                leaderboard.SetEntries(runs);
                
            }
            ));

    }
    private void Update()
    {
        if (Input.anyKey && !notfirst)
        {
            sfx.PlayDelayed(0.4f);
            notfirst = true;
            Fader.StartFade(welcome, next);
           
        }
    }
    // Update is called once per frame

}
