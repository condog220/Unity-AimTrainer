using UnityEngine;

public class PauseScript : MenuManager
{

    public static bool isPaused;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPaused = false;



    }

    // Update is called once per frame
    void Update()
    {
        // The results screen owns the pause state once the round is over.
        if (GameManager.finished)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Pressed");
            PauseGame();
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
