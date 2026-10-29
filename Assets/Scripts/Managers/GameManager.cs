using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static float timer;
    public static bool finished;
    [SerializeField] GameObject timerText;
    [SerializeField] GameObject resultScreen;
    [SerializeField] GameObject scoreResult;
    [SerializeField] GameObject accuracyResult;
    [SerializeField] GameObject highScoreResult;

    private TextMeshProUGUI timerLabel;
    private TextMeshProUGUI scoreLabel;
    private TextMeshProUGUI accuracyLabel;
    private TextMeshProUGUI highScoreLabel;

    private void Awake()
    {
        timer = 30f;
        finished = false;
        timerLabel = timerText.GetComponent<TextMeshProUGUI>();
        scoreLabel = scoreResult.GetComponent<TextMeshProUGUI>();
        accuracyLabel = accuracyResult.GetComponent<TextMeshProUGUI>();
        highScoreLabel = highScoreResult.GetComponent<TextMeshProUGUI>();
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
            timerLabel.text = "0";
            resultsScreen();
            return;
        }
        timerLabel.text = Mathf.CeilToInt(timer).ToString();
    }

    public void resultsScreen()
    {
        finished = true;
        PauseScript.isPaused = true;
        resultScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ScoreManager.instance.SubmitScore();
        scoreLabel.text = ScoreManager.instance.getScore().ToString();
        accuracyLabel.text = ScoreManager.instance.getAccuracy().ToString("F2") + "%";
        highScoreLabel.text = ScoreManager.instance.getHighScore().ToString();

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
