using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    public static float volume = 1.0f;
    public static float sensitivity = 1.0f;
    public static bool randomSize = false;

    private const string VolumeKey = "GameVolume";
    private const string SensitivityKey = "GameSensitivity";
    private const string randomSizeKey = "RandomSize";
    private static SettingsManager instance;

    // Awake (not Start) so a duplicate from returning to the Menu is destroyed before it does anything.
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.SetFloat(SensitivityKey, sensitivity);
        PlayerPrefs.SetInt(randomSizeKey, randomSize ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void LoadSettings()
    {
        volume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);
        sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1.0f);
        randomSize = PlayerPrefs.GetInt(randomSizeKey, 0) == 1;
    }
}
