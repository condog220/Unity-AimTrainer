using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void quitApp()
    {
        Debug.Log("Quit Application");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void playGame()
    {
        ResetState();
        SceneFader.Load("SelectMode");
    }

    public void toMenu()
    {
        ResetState();
        SceneFader.Load("Menu");
    }

    public void toSettings()
    {
        ResetState();
        SceneFader.Load("Settings");
    }

    // The pause and results screens freeze time and free the cursor; never carry that into another scene.
    private static void ResetState()
    {
        Time.timeScale = 1f;
        PauseScript.isPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
