using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelect : MonoBehaviour
{
    private const string LengthKey = "RoundLength";
    public static readonly int[] RoundLengths = { 15, 30, 60 };

    public static GameMode currentMode;
    // Seconds chosen on the length picker. Remembered between sessions.
    public static int roundLength = 30;

    [SerializeField] GameObject lengthPanel;
    [SerializeField] TMP_Text lengthTitle;
    [SerializeField] Button[] lengthButtons;

    private string pendingMode;

    void Start()
    {
        Time.timeScale = 1.0f;
        // Drop any mode left over from the previous round.
        currentMode = null;

        roundLength = PlayerPrefs.GetInt(LengthKey, 30);
        if (System.Array.IndexOf(RoundLengths, roundLength) < 0)
        {
            roundLength = 30;
        }
        lengthPanel.SetActive(false);
        RefreshLengthButtons();
    }

    void Update()
    {
        if (lengthPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePicker();
        }
    }

    public void PickGridShot() { OpenPicker("GridShot"); }

    public void PickTracking() { OpenPicker("Tracking"); }

    public void PickStrafing() { OpenPicker("Strafing"); }

    public void SetLength15() { SetLength(15); }

    public void SetLength30() { SetLength(30); }

    public void SetLength60() { SetLength(60); }

    public void ClosePicker()
    {
        lengthPanel.SetActive(false);
    }

    public void StartRound()
    {
        switch (pendingMode)
        {
            case "Tracking": currentMode = new Tracking(); break;
            case "Strafing": currentMode = new Strafing(); break;
            default: currentMode = new GridShot(); break;
        }
        PlayerPrefs.SetInt(LengthKey, roundLength);
        PlayerPrefs.Save();
        SceneFader.Load("SampleScene");
    }

    private void OpenPicker(string mode)
    {
        pendingMode = mode;
        lengthTitle.text = UITheme.ModeDisplayName(mode).ToUpper();
        RefreshLengthButtons();
        lengthPanel.SetActive(true);
    }

    private void SetLength(int seconds)
    {
        roundLength = seconds;
        RefreshLengthButtons();
    }

    // Selected length gets the accent fill, the others the muted surface fill.
    private void RefreshLengthButtons()
    {
        for (int i = 0; i < lengthButtons.Length; i++)
        {
            bool selected = RoundLengths[i] == roundLength;
            lengthButtons[i].targetGraphic.color = selected ? UITheme.Accent : UITheme.SurfaceHi;
            lengthButtons[i].GetComponentInChildren<TMP_Text>().color = selected ? UITheme.TextOnAccent : UITheme.TextPrimary;
        }
    }
}
