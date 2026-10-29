using TMPro;
using UnityEngine;

// Shows the player's best score for one mode on its select card.
public class ModeCardView : MonoBehaviour
{
    [SerializeField] string modeName;
    [SerializeField] TMP_Text highScoreText;

    void OnEnable()
    {
        ScoreManager.MigrateLegacyScore();
        int best = PlayerPrefs.GetInt(ScoreManager.HighScorePrefix + modeName, 0);
        highScoreText.text = "Best: " + best;
    }
}
