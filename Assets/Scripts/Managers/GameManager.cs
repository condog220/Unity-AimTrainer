using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float timer;
    public static bool finished;
    // True during the 3-2-1 before a round: time is frozen and shooting / looking are locked.
    public static bool counting;
    public static bool InputLocked => PauseScript.isPaused || counting;
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject resultScreen;
    [SerializeField] GameObject pauseHint;
    [SerializeField] TMP_Text scoreResult;
    [SerializeField] TMP_Text accuracyResult;
    [SerializeField] TMP_Text highScoreResult;
    [SerializeField] TMP_Text shotsResult;
    [SerializeField] TMP_Text modeResult;
    [SerializeField] GameObject newRecordBadge;
    [SerializeField] TMP_Text countdownText;

    private const int CountdownFrom = 3;
    private const float GoDuration = 0.5f;
    private float countdownLeft;
    private float goLeft;
    private int lastShown;

    private void Awake()
    {
        timer = GameModeSelect.roundLength;
        finished = false;
        // Locked from Awake so nothing can happen in the gap before Start runs the countdown.
        counting = true;
    }

    void Start()
    {
        BeginCountdown();
    }

    // Freezes the game with timeScale 0, so the countdown below runs on unscaled time.
    private void BeginCountdown()
    {
        counting = true;
        pauseHint.SetActive(false);
        Time.timeScale = 0f;
        countdownLeft = CountdownFrom;
        goLeft = 0f;
        lastShown = 0;
        countdownText.gameObject.SetActive(false);
        timerText.text = Mathf.CeilToInt(timer).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            CountdownToStart();
            return;
        }

        FadeGo();
        CountDownTimer();
    }

    private void CountdownToStart()
    {
        // Clamped because the first frame after a scene load can be a long one.
        countdownLeft -= Mathf.Min(Time.unscaledDeltaTime, 0.1f);

        if (countdownLeft <= 0f)
        {
            // The round starts on "GO!"; the label just fades out while play is already running.
            counting = false;
            Time.timeScale = 1f;
            goLeft = GoDuration;
            countdownText.text = "GO!";
            UIAudio.PlayClick();
            pauseHint.SetActive(true);
            return;
        }

        int shown = Mathf.CeilToInt(countdownLeft);
        if (shown != lastShown)
        {
            lastShown = shown;
            countdownText.gameObject.SetActive(true);
            countdownText.text = shown.ToString();
            UIAudio.PlayClick();
        }
        // countdownLeft is in (shown - 1, shown], so this is 0 -> 1 across each number.
        AnimateBeat(UITheme.TextPrimary, shown - countdownLeft);
    }

    private void FadeGo()
    {
        if (goLeft <= 0f)
        {
            return;
        }

        goLeft -= Time.unscaledDeltaTime;
        if (goLeft <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            return;
        }
        AnimateBeat(UITheme.Accent, 1f - goLeft / GoDuration);
    }

    // t runs 0 -> 1 over one number: pops in large, settles to normal size, fades out.
    private void AnimateBeat(Color color, float t)
    {
        float settle = 1f - Mathf.Pow(1f - Mathf.Min(1f, t * 3f), 3f);
        float scale = Mathf.Lerp(1.6f, 1f, settle);
        countdownText.rectTransform.localScale = new Vector3(scale, scale, 1f);
        color.a = 1f - t * t * t;
        countdownText.color = color;
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
        resultScreen.SetActive(false);

        timer = GameModeSelect.roundLength;
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

        // Sets timeScale itself, so the old `Time.timeScale = 1f` here is no longer needed.
        BeginCountdown();
    }
}
