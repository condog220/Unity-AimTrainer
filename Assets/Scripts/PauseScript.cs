using UnityEngine;

public class PauseScript : MenuManager
{

    public static bool isPaused;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPaused = false;



    }

    // Update is called once per frame
    void Update()
    {
        // The results screen owns the pause state once the round is over; the countdown cannot be paused.
        if (GameManager.finished || GameManager.counting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Pressed");
            // Esc backs out of the settings panel first, then toggles pause.
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.SetActive(isPaused);

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // Pause menu "Settings": swaps the pause card for the settings card and back.
    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsManager.SaveSettings();
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }

    // Pause menu "Restart": close the pause menu, then start a fresh round.
    public void RestartRound()
    {
        if (isPaused)
        {
            PauseGame();
        }
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }


}
