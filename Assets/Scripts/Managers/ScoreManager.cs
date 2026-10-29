using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    private int score = 0;
    private float accuracy = 0;
    private int totalShots = 0;
    private int totalHits = 0;

    [SerializeField] GameObject scoreText;
    [SerializeField] GameObject accuracyText;

    private TextMeshProUGUI scoreLabel;
    private TextMeshProUGUI accuracyLabel;

    private const string LegacyScoreKey = "LastScore";
    private const string HighScorePrefix = "HighScore_";
    private string highScoreKey;

    void Awake()
    {
        instance = this;
        scoreLabel = scoreText.GetComponent<TextMeshProUGUI>();
        accuracyLabel = accuracyText.GetComponent<TextMeshProUGUI>();

        // High scores are per mode; the old single key predates that and is adopted by GridShot once.
        string mode = GameModeSelect.currentMode?.GetType().Name ?? nameof(GridShot);
        highScoreKey = HighScorePrefix + mode;
        string gridShotKey = HighScorePrefix + nameof(GridShot);
        if (PlayerPrefs.HasKey(LegacyScoreKey))
        {
            if (!PlayerPrefs.HasKey(gridShotKey))
            {
                PlayerPrefs.SetInt(gridShotKey, PlayerPrefs.GetInt(LegacyScoreKey));
            }
            PlayerPrefs.DeleteKey(LegacyScoreKey);
            PlayerPrefs.Save();
        }
    }

    public void updateScore()
    {
        score += 200;
        scoreLabel.text = score.ToString();
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
        accuracyLabel.text = accuracy.ToString("F2") + "%";
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
        scoreLabel.text = "0";
        updateAccuracy();
    }

    // Call once when the round ends.
    public void SubmitScore()
    {
        if (score > getHighScore())
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
        }
    }

    public int getHighScore() {
        return PlayerPrefs.GetInt(highScoreKey, 0);
    }


}
