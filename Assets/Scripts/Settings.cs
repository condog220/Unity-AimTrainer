using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] GameObject volumeVal;
    [SerializeField] GameObject sensitivityVal;
    [SerializeField] GameObject randomSizeToggle;
    private float volume;
    private float sensitivity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingsManager.LoadSettings();

        volume = (int)(SettingsManager.volume * 100);
        sensitivity = SettingsManager.sensitivity;

        volumeSlider.value = volume;
        sensitivitySlider.value = sensitivity;
        volumeVal.GetComponent<Text>().text = (volume).ToString("0");
        sensitivityVal.GetComponent<Text>().text = sensitivity.ToString("0.00");
        randomSizeToggle.GetComponent<Toggle>().isOn = SettingsManager.randomSize;

    }

    // Update is called once per frame
    void Update()
    {
        
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
