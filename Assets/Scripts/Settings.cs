using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] GameObject volumeVal;
    [SerializeField] GameObject sensitivityVal;
    [SerializeField] GameObject randomSizeToggle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingsManager.LoadSettings();

        // Sliders are 0-1. Set without notify so opening the scene never overwrites saved values.
        volumeSlider.SetValueWithoutNotify(SettingsManager.volume);
        sensitivitySlider.SetValueWithoutNotify(SettingsManager.sensitivity);
        volumeVal.GetComponent<Text>().text = (SettingsManager.volume * 100).ToString("0");
        sensitivityVal.GetComponent<Text>().text = SettingsManager.sensitivity.ToString("0.00");
        randomSizeToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(SettingsManager.randomSize);

    }

    public void SetVolume(float vol)
    {
        SettingsManager.volume = vol;
        SettingsManager.SaveSettings();       
        volumeVal.GetComponent<Text>().text = (vol * 100).ToString("0");
    }

    public void SetSensitivity(float sens)
    {
        SettingsManager.sensitivity = sens;
        SettingsManager.SaveSettings();
        sensitivityVal.GetComponent<Text>().text = sens.ToString("0.00");
    }

    public void enableRandomSize()
    {
        SettingsManager.randomSize =  randomSizeToggle.GetComponent<Toggle>().isOn;
        SettingsManager.SaveSettings();
    }
}
