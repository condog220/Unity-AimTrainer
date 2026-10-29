using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelect : MonoBehaviour
{

    public static GameMode currentMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1.0f;
        // Drop any mode left over from the previous round.
        currentMode = null;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadGridShot() {
        currentMode = new GridShot();
        SceneManager.LoadScene("SampleScene");

    }

    public void loadTracking()
    {
        currentMode = new Tracking();
        SceneManager.LoadScene("SampleScene");
    }

    public void loadStrafing()
    {
        currentMode = new Strafing();
        SceneManager.LoadScene("SampleScene");
    }
}
