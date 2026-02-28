using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public CanvasGroup PauseMenu;
    public CanvasGroup Settings;
    public CanvasGroup Controls;

    private bool isPaused = false;
    private bool inSettings = false;
    private bool inControls = false;

    void Start()
    {
        PauseMenu.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        Controls.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                // Open pause menu
                isPaused = true;
                PauseMenu.gameObject.SetActive(true);
                Time.timeScale = 0f;
                return;
            }

            // If in Controls → go back to Settings
            if (inControls)
            {
                Debug.Log("Leaving Controls, going back to Settings");
                Controls.gameObject.SetActive(false);
                Settings.gameObject.SetActive(true);
                inControls = false;
                inSettings = true;
                return;
            }

            // If in Settings → go back to Pause
            if (inSettings)
            {
                Settings.gameObject.SetActive(false);
                PauseMenu.gameObject.SetActive(true);
                inSettings = false;
                return;
            }

            // Otherwise → resume game
            Resume();
        }
    }

    public void SettingsMenu()
    {
        Settings.gameObject.SetActive(true);
        PauseMenu.gameObject.SetActive(false);
        inSettings = true;
        inControls = false;
    }

    public void pauseMenuScreen()
    {
        Settings.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(true);
        inSettings = false;
        inControls = false;
        isPaused = true;
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
        PauseMenu.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        Controls.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        inSettings = false;
        inControls = false;
    }

    public void Quit()
    {
        if (DataPersistenceManager.instance != null)
            DataPersistenceManager.instance.SaveGame();

        Application.Quit();
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
}