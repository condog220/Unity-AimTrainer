using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void quitApp()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }

    public void playGame()
    {
        ResetTime();
        SceneManager.LoadScene("SelectMode");
    }

    public void toMenu()
    {
        ResetTime();
        SceneManager.LoadScene("Menu");
    }

    public void toSettings()
    {
        ResetTime();
        SceneManager.LoadScene("Settings");
    }

    // The pause and results screens freeze time; never carry that into another scene.
    private static void ResetTime()
    {
        Time.timeScale = 1f;
        PauseScript.isPaused = false;
    }
}
