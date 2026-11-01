using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    public static float volume = 1.0f;
    public static float sensitivity = 1.0f;
    public const int MinFov = 70;
    public const int MaxFov = 110;
    public static int fov = 90;
    public static bool randomSize = false;

    // Matches the colour on the sphere material, so nothing changes until the player picks one.
    public static readonly Color32 DefaultTargetColor = new Color32(255, 23, 0, 255);
    public static Color32 targetColor = DefaultTargetColor;

    private const string VolumeKey = "GameVolume";
    private const string SensitivityKey = "GameSensitivity";
    private const string FovKey = "GameFov";
    private const string randomSizeKey = "RandomSize";
    private const string TargetColorKey = "TargetColor";
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static MaterialPropertyBlock block;
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
        PlayerPrefs.SetInt(FovKey, fov);
        PlayerPrefs.SetInt(randomSizeKey, randomSize ? 1 : 0);
        PlayerPrefs.SetInt(TargetColorKey, (targetColor.r << 16) | (targetColor.g << 8) | targetColor.b);
        PlayerPrefs.Save();
    }

    public static void LoadSettings()
    {
        volume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);
        sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1.0f);
        fov = Mathf.Clamp(PlayerPrefs.GetInt(FovKey, 90), MinFov, MaxFov);
        randomSize = PlayerPrefs.GetInt(randomSizeKey, 0) == 1;

        int packed = PlayerPrefs.GetInt(TargetColorKey, (DefaultTargetColor.r << 16) | (DefaultTargetColor.g << 8) | DefaultTargetColor.b);
        targetColor = new Color32((byte)((packed >> 16) & 255), (byte)((packed >> 8) & 255), (byte)(packed & 255), 255);
    }

    // Per-renderer override, so the shared material asset is never modified.
    public static void ApplyTargetColor(GameObject target)
    {
        Renderer rend = target.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            return;
        }
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        rend.GetPropertyBlock(block);
        block.SetColor(BaseColorId, targetColor);
        rend.SetPropertyBlock(block);
    }
}
