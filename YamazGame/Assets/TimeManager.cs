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
        if (!countTime || SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 0) return;

        if (timerTextGroup != null) timerTextGroup.alpha = 1f;
        time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        if (timeText != null)
            timeText.text = $"{minutes:00}:{seconds:00}";
        if (time >= 10f)
{
Debug.Log("Running");
    StopTimerAndSubmit();
}
    }

    public void StartNewGame()
    {
        time = 0f;
        countTime = true;
    }


    public void StopTimerAndSubmit()
{
    if (!countTime) return;

    countTime = false;

    long timeInMS = (long)(time * 1000);

    Debug.Log("StopTimerAndSubmit called");
    Debug.Log("BackendManager.Instance = " + BackendManager.Instance);

    if (BackendManager.Instance == null)
    {
        Debug.LogError("BackendManager.Instance is NULL");
        return;
    }

    BackendManager.Instance.SubmitRun(timeInMS, true, () =>
    {
        Debug.Log("Run submitted successfully");
    });
}
}