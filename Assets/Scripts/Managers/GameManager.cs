using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float timer;
    public static bool finished;
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject resultScreen;
    [SerializeField] TMP_Text scoreResult;
    [SerializeField] TMP_Text accuracyResult;
    [SerializeField] TMP_Text highScoreResult;
    [SerializeField] TMP_Text shotsResult;
    [SerializeField] TMP_Text modeResult;
    [SerializeField] GameObject newRecordBadge;

    private void Awake()
    {
        timer = 30f;
        finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        CountDownTimer();

    }

    public void CountDownTimer()
    {
        if (finished)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            timerText.text = "0";
            resultsScreen();
            return;
        }
        timerText.text = Mathf.CeilToInt(timer).ToString();
    }

    public void resultsScreen()
    {
        finished = true;
        PauseScript.isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ScoreManager score = ScoreManager.instance;
        bool newRecord = score.SubmitScore();
        scoreResult.text = score.getScore().ToString();
        accuracyResult.text = score.getAccuracy().ToString("F2") + "%";
        highScoreResult.text = score.getHighScore().ToString();
        if (shotsResult != null)
        {
            shotsResult.text = score.TotalHits + " / " + score.TotalShots;
        }
        if (modeResult != null)
        {
            modeResult.text = UITheme.ModeDisplayName(score.ModeName);
        }
        if (newRecordBadge != null)
        {
            newRecordBadge.SetActive(newRecord);
        }
        resultScreen.SetActive(true);

    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        resultScreen.SetActive(false);

        timer = 30f;
        finished = false;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.resetScore();
        }
        if (AimTrainManager.instance != null)
        {
            AimTrainManager.instance.RestartMode();
        }
        PauseScript.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }
}
