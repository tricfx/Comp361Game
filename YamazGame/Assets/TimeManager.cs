using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private float time;
    private bool countTime;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private CanvasGroup timerTextGroup;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        time = 0f;
        countTime = false;

        if (timerTextGroup != null)
            timerTextGroup.alpha = 0f;
    }

    private void Update()
    {
         int i = SceneManager.GetActiveScene().buildIndex;

        if (i == 0)
        {
            if (timerTextGroup != null)
                timerTextGroup.alpha = 0f;
        }

        if (!countTime || i == 1 || i == 0 || i == 8 || i == 10 || i == 12)
            return;
        if (timerTextGroup != null) timerTextGroup.alpha = 1f;
        time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        if (timeText != null)
            timeText.text = $"{minutes:00}:{seconds:00}";

        // if (Input.GetKeyDown(KeyCode.B))
        // {
        // StopTimerAndSubmit();
        // }
    }

    public void StartNewGame()
    {
        time = 0f;
        countTime = true;
    }

    public void StopTimerAndSubmit()
    {
        countTime = false;

        long timeInMS = (long)(time * 1000);

        if (BackendManager.Instance.SessionManager.AccessToken == null)
        {
            return;
        }
        StartCoroutine(BackendManager.Instance.SubmitRun(timeInMS, true, () =>
        {
            Debug.Log("Run submitted successfully");
        }));
    }
}
