using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // Order matters: UIBuilder builds one swatch button per entry and passes its index to PickSwatch.
    public static readonly Color32[] Swatches =
    {
        new Color32(255, 23, 0, 255),
        new Color32(255, 176, 32, 255),
        new Color32(255, 230, 0, 255),
        new Color32(60, 220, 80, 255),
        new Color32(27, 212, 187, 255),
        new Color32(60, 140, 255, 255),
        new Color32(255, 60, 200, 255),
        new Color32(255, 255, 255, 255),
    };

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider fovSlider;
    [SerializeField] TMP_Text volumeVal;
    [SerializeField] TMP_Text sensitivityVal;
    [SerializeField] TMP_Text fovVal;
    [SerializeField] Toggle randomSizeToggle;

    // Target colour: 0-255 sliders, a preview swatch and a hex readout.
    [SerializeField] Slider redSlider;
    [SerializeField] Slider greenSlider;
    [SerializeField] Slider blueSlider;
    [SerializeField] TMP_Text redVal;
    [SerializeField] TMP_Text greenVal;
    [SerializeField] TMP_Text blueVal;
    [SerializeField] Image colorPreview;
    [SerializeField] TMP_Text hexVal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingsManager.LoadSettings();

        // Sliders are 0-1. Set without notify so opening the scene never overwrites saved values.
        volumeSlider.SetValueWithoutNotify(SettingsManager.volume);
        sensitivitySlider.SetValueWithoutNotify(SettingsManager.sensitivity);
        fovSlider.SetValueWithoutNotify(SettingsManager.fov);
        volumeVal.text = VolumeLabel(SettingsManager.volume);
        sensitivityVal.text = SensitivityLabel(SettingsManager.sensitivity);
        fovVal.text = FovLabel(SettingsManager.fov);
        randomSizeToggle.SetIsOnWithoutNotify(SettingsManager.randomSize);

        SetColourSliders(SettingsManager.targetColor);
        RefreshColourUI();
    }

    private static string VolumeLabel(float vol)
    {
        return (vol * 100).ToString("0") + "%";
    }

    private static string SensitivityLabel(float sens)
    {
        return sens.ToString("0.00");
    }

    private static string FovLabel(int fov)
    {
        return fov + "\u00B0";
    }

    // Sliders only update the in-memory value while dragging; the disk write happens once, when leaving the scene.
    void OnDisable()
    {
        SettingsManager.SaveSettings();
    }

    public void SetVolume(float vol)
    {
        SettingsManager.volume = vol;
        volumeVal.text = VolumeLabel(vol);
    }

    public void SetSensitivity(float sens)
    {
        SettingsManager.sensitivity = sens;
        sensitivityVal.text = SensitivityLabel(sens);
    }

    public void SetRandomSize(bool on)
    {
        SettingsManager.randomSize = on;
        SettingsManager.SaveSettings();
    }

    public void SetFov(float fov)
    {
        SettingsManager.fov = Mathf.Clamp(Mathf.RoundToInt(fov), SettingsManager.MinFov, SettingsManager.MaxFov);
        fovVal.text = FovLabel(SettingsManager.fov);
    }

    // Shared by all three colour sliders; the argument is ignored because all channels are re-read.
    public void SetColour(float _)
    {
        SettingsManager.targetColor = new Color32(
            (byte)Mathf.RoundToInt(redSlider.value),
            (byte)Mathf.RoundToInt(greenSlider.value),
            (byte)Mathf.RoundToInt(blueSlider.value),
            255);
        RefreshColourUI();
    }

    public void PickSwatch(int index)
    {
        SettingsManager.targetColor = Swatches[index];
        SetColourSliders(SettingsManager.targetColor);
        RefreshColourUI();
    }

    private void SetColourSliders(Color32 c)
    {
        redSlider.SetValueWithoutNotify(c.r);
        greenSlider.SetValueWithoutNotify(c.g);
        blueSlider.SetValueWithoutNotify(c.b);
    }

    private void RefreshColourUI()
    {
        Color32 c = SettingsManager.targetColor;
        redVal.text = c.r.ToString();
        greenVal.text = c.g.ToString();
        blueVal.text = c.b.ToString();
        colorPreview.color = c;
        hexVal.text = "#" + ColorUtility.ToHtmlStringRGB(c);
    }
}
