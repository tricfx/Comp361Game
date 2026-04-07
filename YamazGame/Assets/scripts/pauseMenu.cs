using UnityEngine;
using UnityEngine.Audio;

public class pauseMenu : MonoBehaviour
{
    public CanvasGroup PauseMenu;
    public CanvasGroup Settings;
    public CanvasGroup Controls;
    public GameObject BlurOverlay;
    public AudioSource openPause;
    public AudioSource closePause;

    [Header("Pause Audio")]
    public AudioMixer audioMixer;
    public string exposedVolumeParameter = "Music";
    public float pausedVolumeDb = -15f;

    public bool isPaused = false;
    private bool inSettings = false;
    private bool inControls = false;
    private float timeScaleBeforePause = 1f;

    [SerializeField] private LevelLoader levelloader;

    [SerializeField] private SettingsManager settingsManager;


    void Start()
    {
        levelloader = FindFirstObjectByType<LevelLoader>();
        PauseMenu.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        Controls.gameObject.SetActive(false);
        BlurOverlay.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                timeScaleBeforePause = Time.timeScale;
                isPaused = true;
                PauseMenu.gameObject.SetActive(true);
                openPause.Play();
                BlurOverlay.SetActive(true);
                 if (CursorManager.Instance != null)
                CursorManager.Instance.ShowCursor();

                ReduceTargetAudio();
                Time.timeScale = 0f;
                return;
            }

            if (inControls)
            {
                Controls.gameObject.SetActive(false);
                Settings.gameObject.SetActive(true);
                inControls = false;
                inSettings = true;
                return;
            }

            if (inSettings)
            {
                Settings.gameObject.SetActive(false);
                PauseMenu.gameObject.SetActive(true);
                inSettings = false;
                ReduceTargetAudio();
                return;
            }

            Resume();
        }
    }

    public void SettingsMenu()
    {
        Settings.gameObject.SetActive(true);
        PauseMenu.gameObject.SetActive(false);
        inSettings = true;
        inControls = false;

        //RestoreTargetAudio(); //  let settings preview actual volume
    }

    public void pauseMenuScreen()
    {
        if (!isPaused) timeScaleBeforePause = Time.timeScale;
        Settings.gameObject.SetActive(false);
        Controls.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(true);
        BlurOverlay.SetActive(true);

        ReduceTargetAudio();

        inSettings = false;
        inControls = false;
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ControlsMenu()
    {
        Controls.gameObject.SetActive(true);
        Settings.gameObject.SetActive(false);
        inControls = true;
        inSettings = false;
    }

    public void Resume()
    {
        closePause.Play();
        PauseMenu.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        Controls.gameObject.SetActive(false);
        BlurOverlay.SetActive(false);

        RestoreTargetAudio();
         if (CursorManager.Instance != null)
        CursorManager.Instance.HideCursor();

        Time.timeScale = timeScaleBeforePause;
        isPaused = false;
        inSettings = false;
        inControls = false;
    }

    public void Quit()
    {
        if (DataPersistenceManager.instance != null)
            DataPersistenceManager.instance.SaveGame();
            if (BackendManager.Instance.SessionManager.AccessToken != null)
            {
                StartCoroutine(BackendManager.Instance.SignOut(() =>
                {
                    Debug.Log("Sign out successful");
                }));
            }
        Destroy(DataPersistenceManager.instance);
         Time.timeScale = 1f;
       levelloader.LoadLevel(0);
    }

    public void LeavingSettings()
    {
        inSettings = false;
        inControls = true;
    }

    public void LeavingControls()
    {
        inControls = false;
        inSettings = true;
    }

    private void ReduceTargetAudio()
    {
        if (audioMixer == null) return;

        float userDb = ToDb(GameSettingsStore.Load().music);
        audioMixer.SetFloat(exposedVolumeParameter, userDb + pausedVolumeDb);
    }

    private void RestoreTargetAudio()
    {
        if (settingsManager != null)
        {
            settingsManager.SetMusicVolume(GameSettingsStore.Load().music);
        }
        else if (audioMixer != null)
        {
            float userDb = ToDb(GameSettingsStore.Load().music);
            audioMixer.SetFloat(exposedVolumeParameter, userDb);
        }
    }

    private float ToDb(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        return Mathf.Log10(value) * 20f;
    }
}