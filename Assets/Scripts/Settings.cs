using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_Text volumeVal;
    [SerializeField] TMP_Text sensitivityVal;
    [SerializeField] Toggle randomSizeToggle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingsManager.LoadSettings();

        // Sliders are 0-1. Set without notify so opening the scene never overwrites saved values.
        volumeSlider.SetValueWithoutNotify(SettingsManager.volume);
        sensitivitySlider.SetValueWithoutNotify(SettingsManager.sensitivity);
        volumeVal.text = VolumeLabel(SettingsManager.volume);
        sensitivityVal.text = SensitivityLabel(SettingsManager.sensitivity);
        randomSizeToggle.SetIsOnWithoutNotify(SettingsManager.randomSize);

    }

    private static string VolumeLabel(float vol)
    {
        return (vol * 100).ToString("0") + "%";
    }

    private static string SensitivityLabel(float sens)
    {
        return sens.ToString("0.00");
    }

    public void SetVolume(float vol)
    {
        SettingsManager.volume = vol;
        SettingsManager.SaveSettings();
        volumeVal.text = VolumeLabel(vol);
    }

    public void SetSensitivity(float sens)
    {
        SettingsManager.sensitivity = sens;
        SettingsManager.SaveSettings();
        sensitivityVal.text = SensitivityLabel(sens);
    }

    public void SetRandomSize(bool on)
    {
        SettingsManager.randomSize = on;
        SettingsManager.SaveSettings();
    }
}
