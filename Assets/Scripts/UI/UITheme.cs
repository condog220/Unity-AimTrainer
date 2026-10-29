using UnityEngine;

// Single source of truth for UI colours and sizes. Used by the Editor builder and by runtime UI scripts.
public static class UITheme
{
    public static readonly Color BgTop = Hex("0B3F43");
    public static readonly Color BgBottom = Hex("051F23");
    public static readonly Color Surface = Hex("0F4A4E");
    public static readonly Color SurfaceHi = Hex("16666A");
    public static readonly Color Accent = new Color(0.106f, 0.83f, 0.734f);
    public static readonly Color AccentDark = Hex("12A593");
    public static readonly Color TextPrimary = Hex("EAFBF8");
    public static readonly Color TextMuted = Hex("9CC9C4");
    public static readonly Color TextOnAccent = Hex("04302F");
    public static readonly Color Warning = Hex("FFB020");
    public static readonly Color Danger = Hex("FF5A4F");
    public static readonly Color Dim = new Color(0f, 0f, 0f, 0.6f);

    public const float ButtonWidth = 360f;
    public const float ButtonHeight = 72f;

    public static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }

    // Mode class names are the PlayerPrefs / code identifiers; this is what the player sees.
    public static string ModeDisplayName(string modeTypeName)
    {
        switch (modeTypeName)
        {
            case "Tracking": return "TrackShot";
            case "Strafing": return "StrafeShot";
            default: return "GridShot";
        }
    }
}
