using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    public static event Action ScoreChanged;

    public const string HighScorePrefix = "HighScore_";
    private const string LegacyScoreKey = "LastScore";

    private int score = 0;
    private float accuracy = 0;
    private int totalShots = 0;
    private int totalHits = 0;

    // Continuous (TrackShot) scoring: seconds on target and seconds of the round played so far.
    // 1000 per second matches the old 200 per 0.2 s tick, so earlier TrackShot bests stay comparable.
    private const int PointsPerSecond = 1000;
    private const int PulseEvery = 200;
    private float trackedTime = 0;
    private float trackElapsed = 0;

    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text accuracyText;

    private string highScoreKey;

    public string ModeName { get; private set; }
    public int TotalShots => totalShots;
    public int TotalHits => totalHits;
    public bool TimeBased => trackElapsed > 0f;
    public float TrackedTime => trackedTime;
    public float TrackElapsed => trackElapsed;

    void Awake()
    {
        instance = this;
        MigrateLegacyScore();

        // High scores are per mode.
        ModeName = GameModeSelect.currentMode?.GetType().Name ?? nameof(GridShot);
        highScoreKey = HighScorePrefix + ModeName;
    }

    // The old single high-score key predates per-mode scores; GridShot adopts it once.
    public static void MigrateLegacyScore()
    {
        if (!PlayerPrefs.HasKey(LegacyScoreKey))
        {
            return;
        }
        string gridShotKey = HighScorePrefix + nameof(GridShot);
        if (!PlayerPrefs.HasKey(gridShotKey))
        {
            PlayerPrefs.SetInt(gridShotKey, PlayerPrefs.GetInt(LegacyScoreKey));
        }
        PlayerPrefs.DeleteKey(LegacyScoreKey);
        PlayerPrefs.Save();
    }

    public void updateScore()
    {
        score += 200;
        scoreText.text = score.ToString();
        ScoreChanged?.Invoke();
    }

    public void updateAccuracy()
    {
        if(totalShots == 0) {
            accuracy = 0;
        }
        else
        {
            accuracy = ((float)totalHits / (float)totalShots) * 100f;
        }
        accuracyText.text = accuracy.ToString("F2") + "%";
    }

    public void registerShot(bool hit)
    {
        totalShots++;
        if (hit)
        {
            totalHits++;
        }

        updateAccuracy();

    }

    // Call every frame of a continuous round. Accuracy is on-target time over all time played,
    // so not firing counts against it.
    public void AddTrackingTime(float dt, bool onTarget)
    {
        trackElapsed += dt;
        if (onTarget)
        {
            trackedTime += dt;
        }

        int newScore = Mathf.FloorToInt(trackedTime * PointsPerSecond);
        // The HUD "punch" fires every PulseEvery points instead of every frame.
        bool pulse = newScore / PulseEvery != score / PulseEvery;
        if (newScore != score)
        {
            score = newScore;
            scoreText.text = score.ToString();
        }

        accuracy = trackElapsed > 0f ? trackedTime / trackElapsed * 100f : 0f;
        accuracyText.text = accuracy.ToString("F2") + "%";

        if (pulse)
        {
            ScoreChanged?.Invoke();
        }
    }

    public int getScore()
    {
        return score;
    }

    public float getAccuracy()
    {
        return accuracy;
    }

    public void resetScore()
    {
        score = 0;
        totalShots = 0;
        totalHits = 0;
        trackedTime = 0;
        trackElapsed = 0;
        scoreText.text = "0";
        updateAccuracy();
    }

    // Call once when the round ends. Returns true if this score is a new personal best.
    public bool SubmitScore()
    {
        if (score > getHighScore())
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public int getHighScore() {
        return PlayerPrefs.GetInt(highScoreKey, 0);
    }


}
