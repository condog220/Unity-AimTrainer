using TMPro;
using UnityEngine;

// In-game HUD feedback: timer colour/pulse when time is low, score "punch" on each hit.
public class HUDController : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text scoreText;

    private Color timerBase;
    private float punch;

    void Awake()
    {
        timerBase = timerText.color;
    }

    void OnEnable()
    {
        ScoreManager.ScoreChanged += OnScoreChanged;
    }

    void OnDisable()
    {
        ScoreManager.ScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged()
    {
        punch = 1f;
    }

    void Update()
    {
        float t = GameManager.timer;
        Color color = timerBase;
        float scale = 1f;

        if (!GameManager.finished)
        {
            if (t <= 5f)
            {
                color = UITheme.Danger;
                scale = 1f + 0.08f * Mathf.Abs(Mathf.Sin(Time.unscaledTime * 6f));
            }
            else if (t <= 10f)
            {
                color = UITheme.Warning;
            }
        }
        timerText.color = color;
        timerText.rectTransform.localScale = new Vector3(scale, scale, 1f);

        if (punch > 0f)
        {
            punch = Mathf.Max(0f, punch - Time.unscaledDeltaTime * 4f);
            float s = 1f + 0.25f * punch;
            scoreText.rectTransform.localScale = new Vector3(s, s, 1f);
        }
    }
}
