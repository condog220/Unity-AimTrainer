using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Regenerates the Canvas UI of every scene from code (AimTrain > Rebuild UI). Re-run it any time to reset the UI.
// Only root Canvas objects are replaced; managers, cameras and the EventSystem are kept and re-wired.
public static class UIBuilder
{
    private const string SceneDir = "Assets/Scenes/";
    private const string UIDir = "Assets/UI/";
    private const string ControlsHint = "Aim: Mouse      Shoot: Left Click      Strafe: A / D (StrafeShot)      Pause: Esc";

    private static readonly string[] SceneNames = { "Menu", "SelectMode", "Settings", "SampleScene" };

    private static Sprite sRounded, sCircle, sGlow, sGradient, sTracking;

    // ------------------------------------------------------------------ menu items

    [MenuItem("AimTrain/Rebuild UI/All Scenes")]
    private static void MenuAll()
    {
        if (!CanRun()) return;
        foreach (string name in SceneNames) Build(name);
        Debug.Log("[UIBuilder] Rebuilt all scenes. Run AimTrain > Validate UI to check the wiring.");
    }

    [MenuItem("AimTrain/Rebuild UI/Menu")]
    private static void MenuMenu() { if (CanRun()) Build("Menu"); }

    [MenuItem("AimTrain/Rebuild UI/Select Mode")]
    private static void MenuSelect() { if (CanRun()) Build("SelectMode"); }

    [MenuItem("AimTrain/Rebuild UI/Settings")]
    private static void MenuSettings() { if (CanRun()) Build("Settings"); }

    [MenuItem("AimTrain/Rebuild UI/Game (HUD, Pause, Results)")]
    private static void MenuGame() { if (CanRun()) Build("SampleScene"); }

    [MenuItem("AimTrain/Validate UI")]
    private static void MenuValidate()
    {
        if (!CanRun()) return;
        int errors = 0;
        foreach (string name in SceneNames) errors += Validate(name);
        string msg = errors == 0 ? "All scenes valid: every reference is assigned and every button is wired." : errors + " problem(s) found. See the Console.";
        Debug.Log("[UIBuilder] " + msg);
        EditorUtility.DisplayDialog("AimTrain UI", msg, "OK");
    }

    private static bool CanRun()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("AimTrain UI", "Exit Play mode first.", "OK");
            return false;
        }
        return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }

    // ------------------------------------------------------------------ scene flow

    private static void Build(string sceneName)
    {
        try
        {
            Scene scene = EditorSceneManager.OpenScene(SceneDir + sceneName + ".unity", OpenSceneMode.Single);
            EnsureSprites();

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.GetComponent<Canvas>() != null)
                {
                    Object.DestroyImmediate(root);
                }
            }

            switch (sceneName)
            {
                case "Menu": BuildMenu(); break;
                case "SelectMode": BuildSelectMode(); break;
                case "Settings": BuildSettings(); break;
                case "SampleScene": BuildGame(); break;
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[UIBuilder] Rebuilt " + sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[UIBuilder] Failed to rebuild " + sceneName + ": " + e);
        }
    }

    // ------------------------------------------------------------------ Menu

    private static void BuildMenu()
    {
        MenuManager menu = Need<MenuManager>();
        RectTransform root = NewCanvas("Canvas");
        Backdrop(root);

        RectTransform column = NewRect("Column", root);
        Place(column, new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(700f, 100f));
        VStack(column, 18f, TextAnchor.MiddleCenter);
        FitContent(column);

        TextMeshProUGUI title = Txt("Title", column, "AIM TRAINER", 132f, UITheme.TextPrimary, FontStyles.Bold);
        LE(title.gameObject, h: 150f);
        Image bar = Img("AccentBar", column, UITheme.Accent, sRounded, true);
        LE(bar.gameObject, 140f, 8f);
        TextMeshProUGUI sub = Txt("Subtitle", column, "30-second aim drills. Pick a mode and go.", 34f, UITheme.TextMuted);
        LE(sub.gameObject, h: 50f);
        Spacer(column, 48f);

        Wire(Btn("PlayButton", column, "PLAY", new Vector2(440f, 88f), true, 40f), menu.playGame);
        Wire(Btn("SettingsButton", column, "SETTINGS", new Vector2(440f, 80f), false), menu.toSettings);
        Wire(Btn("QuitButton", column, "QUIT", new Vector2(440f, 80f), false), menu.quitApp);

        TextMeshProUGUI hint = Txt("ControlsHint", root, ControlsHint, 26f, UITheme.TextMuted);
        Place(hint.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(1500f, 40f));

        TextMeshProUGUI version = Txt("Version", root, "v" + PlayerSettings.bundleVersion, 22f, UITheme.TextMuted, FontStyles.Normal, TextAlignmentOptions.Right);
        Place(version.rectTransform, new Vector2(1f, 0f), new Vector2(-36f, 36f), new Vector2(240f, 36f));
    }

    // ------------------------------------------------------------------ Select Mode

    private static void BuildSelectMode()
    {
        GameModeSelect select = Need<GameModeSelect>();
        MenuManager menu = NeedOrCreate<MenuManager>("MenuManager");
        RectTransform root = NewCanvas("Canvas");
        Backdrop(root);

        TextMeshProUGUI title = Txt("Title", root, "SELECT MODE", 84f, UITheme.TextPrimary, FontStyles.Bold);
        Place(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -50f), new Vector2(1200f, 120f));
        TextMeshProUGUI sub = Txt("Subtitle", root, "Every round lasts 30 seconds. Your best score is saved per mode.", 30f, UITheme.TextMuted);
        Place(sub.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -170f), new Vector2(1400f, 44f));

        RectTransform row = NewRect("Cards", root);
        Place(row, new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(1600f, 700f));
        HStack(row, 48f, TextAnchor.MiddleCenter);
        FitContent(row);

        Sprite grid = LoadSprite("Assets/Images/gridshot.png");
        Sprite strafe = LoadSprite("Assets/Images/Circlestrafing_animation.png");
        ModeCard(row, "GridShot", "Pop three targets on a fixed grid. Click each one as fast as you can.", grid, "GridShot", select.loadGridShot);
        ModeCard(row, "TrackShot", "Hold fire and keep your crosshair glued to a moving target.", sTracking, "Tracking", select.loadTracking);
        ModeCard(row, "StrafeShot", "Strafe with A / D while flicking to targets around the arena.", strafe, "Strafing", select.loadStrafing);

        Button back = Btn("BackButton", root, "< Back", new Vector2(220f, 70f), false, 30f);
        Place(back.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(60f, 50f), new Vector2(220f, 70f));
        Wire(back, menu.toMenu);
    }

    private static void ModeCard(Transform row, string title, string description, Sprite preview, string modeKey, UnityAction onClick)
    {
        Image card = Img(title + "Card", row, UITheme.Surface, sRounded, true, true);
        LE(card.gameObject, 500f, 700f);
        card.rectTransform.sizeDelta = new Vector2(500f, 700f);
        VStack(card.rectTransform, 14f, TextAnchor.UpperCenter, 24, true);
        AddShadow(card.gameObject);

        Button button = card.gameObject.AddComponent<Button>();
        button.targetGraphic = card;
        Tint(button);
        AddFx(card.gameObject, 1.04f);

        Image frame = Img("Preview", card.transform, UITheme.BgBottom, sRounded, true);
        LE(frame.gameObject, h: 260f);
        if (preview != null)
        {
            Image pic = Img("Image", frame.transform, Color.white, preview);
            Fill(pic.rectTransform, 10f, 10f, 10f, 10f);
            pic.preserveAspect = true;
        }

        TextMeshProUGUI name = Txt("Name", card.transform, title, 46f, UITheme.TextPrimary, FontStyles.Bold);
        LE(name.gameObject, h: 60f);
        TextMeshProUGUI desc = Txt("Description", card.transform, description, 26f, UITheme.TextMuted);
        LE(desc.gameObject, h: 110f);
        TextMeshProUGUI best = Txt("Best", card.transform, "Best: 0", 30f, UITheme.Accent, FontStyles.Bold);
        LE(best.gameObject, h: 44f);

        RectTransform flex = NewRect("Flex", card.transform);
        LE(flex.gameObject, fh: 1f);

        Image pill = Img("PlayPill", card.transform, UITheme.Accent, sRounded, true);
        LE(pill.gameObject, h: 64f);
        Fill(Txt("Label", pill.transform, "PLAY", 32f, UITheme.TextOnAccent, FontStyles.Bold).rectTransform);

        ModeCardView view = card.gameObject.AddComponent<ModeCardView>();
        AssignString(view, "modeName", modeKey);
        Assign(view, "highScoreText", best);

        Wire(button, onClick);
    }

    // ------------------------------------------------------------------ Settings

    private static void BuildSettings()
    {
        Settings settings = Need<Settings>();
        MenuManager menu = Need<MenuManager>();
        RectTransform root = NewCanvas("Canvas");
        Backdrop(root);

        Image card = Img("Card", root, UITheme.Surface, sRounded, true);
        Place(card.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100f, 100f));
        VStack(card.rectTransform, 30f, TextAnchor.UpperCenter, 56);
        FitContent(card.rectTransform);
        AddShadow(card.gameObject);

        TextMeshProUGUI title = Txt("Title", card.transform, "SETTINGS", 72f, UITheme.TextPrimary, FontStyles.Bold);
        LE(title.gameObject, h: 90f);

        Slider volume = SliderRow(card.transform, "Volume", out TextMeshProUGUI volumeValue, "100%");
        Slider sensitivity = SliderRow(card.transform, "Sensitivity", out TextMeshProUGUI sensitivityValue, "1.00");

        RectTransform toggleRow = NewRect("RandomSizeRow", card.transform);
        LE(toggleRow.gameObject, 960f, 64f);
        HStack(toggleRow, 24f, TextAnchor.MiddleLeft);
        TextMeshProUGUI toggleLabel = Txt("Label", toggleRow, "Random target size", 36f, UITheme.TextPrimary, FontStyles.Normal, TextAlignmentOptions.Left);
        LE(toggleLabel.gameObject, fw: 1f);
        Toggle randomSize = NewToggle(toggleRow);

        Image divider = Img("Divider", card.transform, new Color(1f, 1f, 1f, 0.12f), null);
        LE(divider.gameObject, 960f, 2f);

        TextMeshProUGUI controlsTitle = Txt("ControlsTitle", card.transform, "CONTROLS", 24f, UITheme.TextMuted, FontStyles.Bold);
        LE(controlsTitle.gameObject, h: 32f);
        TextMeshProUGUI controls = Txt("Controls", card.transform,
            "Mouse  -  aim\nLeft click  -  shoot\nA / D  -  strafe (StrafeShot)\nEsc  -  pause", 28f, UITheme.TextPrimary);
        LE(controls.gameObject, h: 150f);

        Button back = Btn("BackButton", card.transform, "BACK", new Vector2(360f, 76f), true);

        Assign(settings, "volumeSlider", volume);
        Assign(settings, "sensitivitySlider", sensitivity);
        Assign(settings, "volumeVal", volumeValue);
        Assign(settings, "sensitivityVal", sensitivityValue);
        Assign(settings, "randomSizeToggle", randomSize);

        UnityEventTools.AddPersistentListener(volume.onValueChanged, settings.SetVolume);
        UnityEventTools.AddPersistentListener(sensitivity.onValueChanged, settings.SetSensitivity);
        UnityEventTools.AddPersistentListener(randomSize.onValueChanged, settings.SetRandomSize);
        Wire(back, menu.toMenu);
    }

    private static Slider SliderRow(Transform parent, string label, out TextMeshProUGUI valueText, string initial)
    {
        RectTransform row = NewRect(label + "Row", parent);
        LE(row.gameObject, 960f, 64f);
        HStack(row, 24f, TextAnchor.MiddleCenter);

        TextMeshProUGUI name = Txt("Label", row, label, 36f, UITheme.TextPrimary, FontStyles.Normal, TextAlignmentOptions.Left);
        LE(name.gameObject, 300f);
        Slider slider = NewSlider(row, label + "Slider", new Vector2(460f, 56f));
        valueText = Txt("Value", row, initial, 34f, UITheme.Accent, FontStyles.Bold, TextAlignmentOptions.Right);
        LE(valueText.gameObject, 140f);
        return slider;
    }

    private static Slider NewSlider(Transform parent, string name, Vector2 size)
    {
        RectTransform root = NewRect(name, parent);
        root.sizeDelta = size;
        LE(root.gameObject, size.x, size.y);

        Image track = Img("Background", root, new Color(0f, 0f, 0f, 0.35f), sRounded, true, true);
        track.rectTransform.anchorMin = new Vector2(0f, 0.5f);
        track.rectTransform.anchorMax = new Vector2(1f, 0.5f);
        track.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        track.rectTransform.sizeDelta = new Vector2(0f, 14f);

        RectTransform fillArea = NewRect("Fill Area", root);
        fillArea.anchorMin = new Vector2(0f, 0.5f);
        fillArea.anchorMax = new Vector2(1f, 0.5f);
        fillArea.pivot = new Vector2(0.5f, 0.5f);
        fillArea.offsetMin = new Vector2(0f, -7f);
        fillArea.offsetMax = new Vector2(0f, 7f);
        Image fill = Img("Fill", fillArea, UITheme.Accent, sRounded, true);
        fill.rectTransform.anchorMin = Vector2.zero;
        fill.rectTransform.anchorMax = new Vector2(0f, 1f);
        fill.rectTransform.sizeDelta = new Vector2(14f, 0f);

        RectTransform handleArea = NewRect("Handle Slide Area", root);
        Fill(handleArea, 20f, 20f, 0f, 0f);
        Image handle = Img("Handle", handleArea, Color.white, sCircle, false, true);
        handle.rectTransform.anchorMin = new Vector2(0f, 0f);
        handle.rectTransform.anchorMax = new Vector2(0f, 1f);
        handle.rectTransform.sizeDelta = new Vector2(40f, 0f);
        AddShadow(handle.gameObject);

        Slider slider = root.gameObject.AddComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        AddFx(handle.gameObject, 1.15f);
        return slider;
    }

    private static Toggle NewToggle(Transform parent)
    {
        Image bg = Img("RandomSizeToggle", parent, new Color(0f, 0f, 0f, 0.35f), sRounded, true, true);
        LE(bg.gameObject, 56f, 56f);
        bg.rectTransform.sizeDelta = new Vector2(56f, 56f);
        Image check = Img("Checkmark", bg.transform, UITheme.Accent, LoadBuiltin("UI/Skin/Checkmark.psd"));
        Fill(check.rectTransform, 8f, 8f, 8f, 8f);

        Toggle toggle = bg.gameObject.AddComponent<Toggle>();
        toggle.targetGraphic = bg;
        toggle.graphic = check;
        toggle.isOn = false;
        AddFx(bg.gameObject, 1.1f);
        return toggle;
    }

    // ------------------------------------------------------------------ Game (SampleScene)

    private static void BuildGame()
    {
        GameManager gm = Need<GameManager>();
        ScoreManager sm = Need<ScoreManager>();
        PauseScript pause = Need<PauseScript>();

        RectTransform root = NewCanvas("Canvas");

        // HUD
        StatPill(root, "ScorePill", "SCORE", "0", new Vector2(0f, 1f), new Vector2(48f, -40f), new Vector2(280f, 118f), out TextMeshProUGUI scoreValue);
        StatPill(root, "TimerPill", "TIME", "30", new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(220f, 118f), out TextMeshProUGUI timerValue);
        StatPill(root, "AccuracyPill", "ACCURACY", "0.00%", new Vector2(1f, 1f), new Vector2(-48f, -40f), new Vector2(320f, 118f), out TextMeshProUGUI accuracyValue);

        TextMeshProUGUI pauseHint = Txt("PauseHint", root, "Esc: pause", 22f, new Color(UITheme.TextMuted.r, UITheme.TextMuted.g, UITheme.TextMuted.b, 0.7f));
        Place(pauseHint.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 24f), new Vector2(400f, 34f));

        Crosshair crosshair = BuildCrosshair(root);

        HUDController hud = root.gameObject.AddComponent<HUDController>();
        Assign(hud, "timerText", timerValue);
        Assign(hud, "scoreText", scoreValue);

        // Pause
        RectTransform pauseCard;
        GameObject pausePanel = Overlay(root, "PausePanel", 640f, out pauseCard);
        Txt("Title", pauseCard, "PAUSED", 72f, UITheme.TextPrimary, FontStyles.Bold);
        Spacer(pauseCard, 6f);
        Button resume = Btn("ResumeButton", pauseCard, "RESUME", new Vector2(440f, 84f), true, 36f);
        Button restart = Btn("RestartButton", pauseCard, "RESTART", new Vector2(440f, 76f), false);
        Button menuButton = Btn("MenuButton", pauseCard, "MAIN MENU", new Vector2(440f, 76f), false);
        TextMeshProUGUI pauseControls = Txt("Hint", pauseCard, "Esc: resume", 24f, UITheme.TextMuted);
        LE(pauseControls.gameObject, h: 34f);
        Wire(resume, pause.PauseGame);
        Wire(restart, pause.RestartRound);
        Wire(menuButton, pause.toMenu);
        pausePanel.SetActive(false);

        // Results
        RectTransform resultsCard;
        GameObject resultsPanel = Overlay(root, "ResultsPanel", 760f, out resultsCard);
        TextMeshProUGUI resultsTitle = Txt("Title", resultsCard, "ROUND COMPLETE", 64f, UITheme.TextPrimary, FontStyles.Bold);
        LE(resultsTitle.gameObject, h: 80f);
        TextMeshProUGUI modeText = Txt("Mode", resultsCard, "GridShot", 38f, UITheme.Accent, FontStyles.Bold);
        LE(modeText.gameObject, h: 50f);

        Image badge = Img("NewRecordBadge", resultsCard, UITheme.Warning, sRounded, true);
        LE(badge.gameObject, 300f, 54f);
        Fill(Txt("Label", badge.transform, "NEW RECORD!", 30f, UITheme.TextOnAccent, FontStyles.Bold).rectTransform);
        badge.gameObject.SetActive(false);

        StatRow(resultsCard, "Score", out TextMeshProUGUI scoreResult);
        StatRow(resultsCard, "Accuracy", out TextMeshProUGUI accuracyResult);
        StatRow(resultsCard, "Shots (hits / total)", out TextMeshProUGUI shotsResult);
        StatRow(resultsCard, "High score", out TextMeshProUGUI highScoreResult);

        Spacer(resultsCard, 10f);
        RectTransform buttons = NewRect("Buttons", resultsCard);
        LE(buttons.gameObject, h: 84f);
        HStack(buttons, 24f, TextAnchor.MiddleCenter);
        Button replay = Btn("ReplayButton", buttons, "REPLAY", new Vector2(300f, 80f), true, 34f);
        Button resultsMenu = Btn("MenuButton", buttons, "MENU", new Vector2(300f, 80f), false, 34f);
        Wire(replay, gm.RestartGame);
        Wire(resultsMenu, pause.toMenu);
        resultsPanel.SetActive(false);

        // Script fields
        Assign(sm, "scoreText", scoreValue);
        Assign(sm, "accuracyText", accuracyValue);

        Assign(gm, "timerText", timerValue);
        Assign(gm, "resultScreen", resultsPanel);
        Assign(gm, "scoreResult", scoreResult);
        Assign(gm, "accuracyResult", accuracyResult);
        Assign(gm, "highScoreResult", highScoreResult);
        Assign(gm, "shotsResult", shotsResult);
        Assign(gm, "modeResult", modeText);
        Assign(gm, "newRecordBadge", badge.gameObject);

        Assign(pause, "pauseMenu", pausePanel);
        Assign(pause, "gameManager", gm);
    }

    private static void StatPill(Transform parent, string name, string label, string value, Vector2 anchor, Vector2 pos, Vector2 size, out TextMeshProUGUI valueText)
    {
        Image bg = Img(name, parent, new Color(UITheme.BgBottom.r, UITheme.BgBottom.g, UITheme.BgBottom.b, 0.72f), sRounded, true);
        Place(bg.rectTransform, anchor, pos, size);
        VStack(bg.rectTransform, 0f, TextAnchor.MiddleCenter, 8);
        Txt("Label", bg.transform, label, 22f, UITheme.TextMuted, FontStyles.Bold);
        valueText = Txt("Value", bg.transform, value, 54f, UITheme.TextPrimary, FontStyles.Bold);
    }

    private static Crosshair BuildCrosshair(Transform canvas)
    {
        RectTransform root = NewRect("Crosshair", canvas);
        Place(root, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 80f));
        Color c = new Color(1f, 1f, 1f, 0.9f);

        Place(Img("Dot", root, c, sCircle).rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(6f, 6f));
        Place(Img("Left", root, c).rectTransform, new Vector2(0.5f, 0.5f), new Vector2(-16f, 0f), new Vector2(14f, 3f));
        Place(Img("Right", root, c).rectTransform, new Vector2(0.5f, 0.5f), new Vector2(16f, 0f), new Vector2(14f, 3f));
        Place(Img("Up", root, c).rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 16f), new Vector2(3f, 14f));
        Place(Img("Down", root, c).rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -16f), new Vector2(3f, 14f));

        RectTransform marker = NewRect("HitMarker", root);
        Place(marker, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 80f));
        CanvasGroup group = marker.gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.interactable = false;
        float d = 18f;
        Color hit = UITheme.Accent;
        DiagonalBar(marker, new Vector2(-d, d), -45f, hit);
        DiagonalBar(marker, new Vector2(d, d), 45f, hit);
        DiagonalBar(marker, new Vector2(-d, -d), 45f, hit);
        DiagonalBar(marker, new Vector2(d, -d), -45f, hit);

        Crosshair crosshair = root.gameObject.AddComponent<Crosshair>();
        Assign(crosshair, "hitMarker", group);
        return crosshair;
    }

    private static void DiagonalBar(Transform parent, Vector2 pos, float angle, Color color)
    {
        RectTransform rt = Img("Bar", parent, color).rectTransform;
        Place(rt, new Vector2(0.5f, 0.5f), pos, new Vector2(18f, 4f));
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Dimmed full-screen overlay holding a centred card. Returns the overlay; `card` is the vertical stack to fill.
    private static GameObject Overlay(Transform canvas, string name, float cardWidth, out RectTransform card)
    {
        Image dim = Img(name, canvas, UITheme.Dim, null, false, true);
        Fill(dim.rectTransform);
        dim.gameObject.AddComponent<CanvasGroup>();

        Image cardImage = Img("Card", dim.transform, UITheme.Surface, sRounded, true);
        Place(cardImage.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(cardWidth, 100f));
        VStack(cardImage.rectTransform, 22f, TextAnchor.UpperCenter, 48);
        FitContent(cardImage.rectTransform);
        AddShadow(cardImage.gameObject);

        UIPanel panel = dim.gameObject.AddComponent<UIPanel>();
        Assign(panel, "content", cardImage.rectTransform);
        card = cardImage.rectTransform;
        return dim.gameObject;
    }

    private static void StatRow(Transform parent, string label, out TextMeshProUGUI valueText)
    {
        RectTransform row = NewRect(label + "Row", parent);
        LE(row.gameObject, 620f, 52f);
        HStack(row, 20f, TextAnchor.MiddleCenter);
        TextMeshProUGUI name = Txt("Label", row, label, 32f, UITheme.TextMuted, FontStyles.Normal, TextAlignmentOptions.Left);
        LE(name.gameObject, fw: 1f);
        valueText = Txt("Value", row, "0", 38f, UITheme.TextPrimary, FontStyles.Bold, TextAlignmentOptions.Right);
        LE(valueText.gameObject, 240f);
    }

    // ------------------------------------------------------------------ shared building blocks

    private static RectTransform NewCanvas(string name)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return (RectTransform)go.transform;
    }

    private static void Backdrop(Transform canvas)
    {
        Image bg = Img("Background", canvas, Color.white, sGradient);
        Fill(bg.rectTransform);
        Color glow = new Color(UITheme.Accent.r, UITheme.Accent.g, UITheme.Accent.b, 0.16f);
        Place(Img("GlowA", canvas, glow, sGlow).rectTransform, new Vector2(0f, 1f), Vector2.zero, new Vector2(1500f, 1500f));
        Place(Img("GlowB", canvas, glow, sGlow).rectTransform, new Vector2(1f, 0f), Vector2.zero, new Vector2(1500f, 1500f));
    }

    private static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static void Fill(RectTransform rt, float left = 0f, float right = 0f, float top = 0f, float bottom = 0f)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    // Anchors to a point and pivots on the same point, so `pos` is the offset from that screen edge/corner/centre.
    private static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    private static Image Img(string name, Transform parent, Color color, Sprite sprite = null, bool sliced = false, bool raycast = false)
    {
        RectTransform rt = NewRect(name, parent);
        Image image = rt.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = raycast;
        image.type = (sliced && sprite != null) ? Image.Type.Sliced : Image.Type.Simple;
        return image;
    }

    private static TextMeshProUGUI Txt(string name, Transform parent, string text, float size, Color color,
        FontStyles style = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        RectTransform rt = NewRect(name, parent);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.font = TMP_Settings.defaultFontAsset;
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.fontStyle = style;
        t.alignment = align;
        t.raycastTarget = false;
        return t;
    }

    private static LayoutElement LE(GameObject go, float w = -1f, float h = -1f, float fw = -1f, float fh = -1f)
    {
        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        if (w >= 0f) le.preferredWidth = w;
        if (h >= 0f) le.preferredHeight = h;
        if (fw >= 0f) le.flexibleWidth = fw;
        if (fh >= 0f) le.flexibleHeight = fh;
        return le;
    }

    private static void Spacer(Transform parent, float height)
    {
        LE(NewRect("Spacer", parent).gameObject, h: height);
    }

    private static VerticalLayoutGroup VStack(RectTransform rt, float spacing, TextAnchor align, int pad = 0, bool expandWidth = false)
    {
        VerticalLayoutGroup v = rt.gameObject.AddComponent<VerticalLayoutGroup>();
        v.spacing = spacing;
        v.childAlignment = align;
        v.padding = new RectOffset(pad, pad, pad, pad);
        v.childControlWidth = true;
        v.childControlHeight = true;
        v.childForceExpandWidth = expandWidth;
        v.childForceExpandHeight = false;
        return v;
    }

    private static HorizontalLayoutGroup HStack(RectTransform rt, float spacing, TextAnchor align, int pad = 0)
    {
        HorizontalLayoutGroup h = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
        h.spacing = spacing;
        h.childAlignment = align;
        h.padding = new RectOffset(pad, pad, pad, pad);
        h.childControlWidth = true;
        h.childControlHeight = true;
        h.childForceExpandWidth = false;
        h.childForceExpandHeight = false;
        return h;
    }

    private static void FitContent(RectTransform rt)
    {
        ContentSizeFitter fitter = rt.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static void AddShadow(GameObject go)
    {
        Shadow s = go.AddComponent<Shadow>();
        s.effectColor = new Color(0f, 0f, 0f, 0.35f);
        s.effectDistance = new Vector2(0f, -5f);
    }

    private static void Tint(Button button)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(0.88f, 0.88f, 0.88f, 1f);
        cb.highlightedColor = Color.white;
        cb.selectedColor = Color.white;
        cb.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
        cb.disabledColor = new Color(1f, 1f, 1f, 0.4f);
        cb.fadeDuration = 0.08f;
        button.colors = cb;
    }

    private static void AddFx(GameObject go, float hoverScale)
    {
        UIButtonFX fx = go.AddComponent<UIButtonFX>();
        SerializedObject so = new SerializedObject(fx);
        so.FindProperty("hoverScale").floatValue = hoverScale;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Button Btn(string name, Transform parent, string label, Vector2 size, bool primary, float fontSize = 32f)
    {
        Image image = Img(name, parent, primary ? UITheme.Accent : UITheme.SurfaceHi, sRounded, true, true);
        image.rectTransform.sizeDelta = size;
        LE(image.gameObject, size.x, size.y);

        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        Tint(button);
        AddShadow(image.gameObject);
        image.gameObject.AddComponent<UIButtonFX>();

        TextMeshProUGUI text = Txt("Label", image.transform, label, fontSize, primary ? UITheme.TextOnAccent : UITheme.TextPrimary, FontStyles.Bold);
        Fill(text.rectTransform);
        return button;
    }

    private static void Wire(Button button, UnityAction action)
    {
        if (action == null || action.Target == null)
        {
            Debug.LogError("[UIBuilder] Missing target for button " + button.name);
            return;
        }
        UnityEventTools.AddPersistentListener(button.onClick, action);
    }

    private static T Need<T>() where T : Component
    {
        T found = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new System.InvalidOperationException("No " + typeof(T).Name + " in scene " + SceneManager.GetActiveScene().name);
        }
        return found;
    }

    private static T NeedOrCreate<T>(string objectName) where T : Component
    {
        T found = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (found != null)
        {
            return found;
        }
        return new GameObject(objectName).AddComponent<T>();
    }

    private static void Assign(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty(field);
        if (property == null)
        {
            Debug.LogError("[UIBuilder] " + target.GetType().Name + " has no serialized field '" + field + "'");
            return;
        }
        property.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignString(Object target, string field, string value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty(field);
        if (property == null)
        {
            Debug.LogError("[UIBuilder] " + target.GetType().Name + " has no serialized field '" + field + "'");
            return;
        }
        property.stringValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning("[UIBuilder] Missing sprite " + path);
        }
        return sprite;
    }

    private static Sprite LoadBuiltin(string path)
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>(path);
    }

    // ------------------------------------------------------------------ generated sprites

    private static void EnsureSprites()
    {
        Directory.CreateDirectory("Assets/UI");
        sRounded = MakeSprite("rounded.png", 64, 64, (x, y) => RoundedPixel(x, y, 64, 64, 24f), new Vector4(26f, 26f, 26f, 26f));
        sCircle = MakeSprite("circle.png", 64, 64, (x, y) => RoundedPixel(x, y, 64, 64, 32f), Vector4.zero);
        sGlow = MakeSprite("glow.png", 256, 256, GlowPixel, Vector4.zero);
        sGradient = MakeSprite("bg_gradient.png", 4, 256, (x, y) => Color.Lerp(UITheme.BgBottom, UITheme.BgTop, y / 255f), Vector4.zero);
        sTracking = MakeSprite("preview_tracking.png", 512, 288, TrackingPixel, Vector4.zero);
    }

    // Existing files are reused; delete a PNG in Assets/UI to regenerate it.
    private static Sprite MakeSprite(string file, int width, int height, System.Func<int, int, Color> pixel, Vector4 border)
    {
        string path = UIDir + file;
        Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null)
        {
            return existing;
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = pixel(x, y);
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spriteBorder = border;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // White rounded rectangle with a 1px anti-aliased edge (signed distance field).
    private static Color RoundedPixel(int x, int y, int w, int h, float radius)
    {
        Vector2 p = new Vector2(x + 0.5f - w * 0.5f, y + 0.5f - h * 0.5f);
        Vector2 half = new Vector2(w * 0.5f - radius, h * 0.5f - radius);
        Vector2 q = new Vector2(Mathf.Abs(p.x) - half.x, Mathf.Abs(p.y) - half.y);
        float d = new Vector2(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f)).magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - radius;
        return new Color(1f, 1f, 1f, Mathf.Clamp01(0.5f - d));
    }

    private static Color GlowPixel(int x, int y)
    {
        float dx = (x - 127.5f) / 128f;
        float dy = (y - 127.5f) / 128f;
        float r = Mathf.Sqrt(dx * dx + dy * dy);
        return new Color(1f, 1f, 1f, Mathf.Pow(Mathf.Clamp01(1f - r), 2.2f));
    }

    // Stand-in preview for TrackShot: a target with a motion trail.
    private static Color TrackingPixel(int x, int y)
    {
        Color col = Color.Lerp(UITheme.BgBottom, UITheme.Surface, y / 287f);
        Vector2 p = new Vector2(x, y);
        Vector2 center = new Vector2(340f, 150f);

        for (int k = 1; k <= 4; k++)
        {
            Vector2 trail = center + new Vector2(-k * 58f, Mathf.Sin(k * 0.9f) * 10f);
            if (Vector2.Distance(p, trail) < 40f - k * 6f)
            {
                col = Color.Lerp(col, UITheme.Accent, 0.55f - k * 0.1f);
            }
        }

        float dist = Vector2.Distance(p, center);
        if (dist < 48f)
        {
            col = (Mathf.FloorToInt(dist / 16f) % 2 == 0) ? UITheme.Accent : UITheme.TextPrimary;
        }
        return col;
    }

    // ------------------------------------------------------------------ validation

    private static int Validate(string sceneName)
    {
        Scene scene = EditorSceneManager.OpenScene(SceneDir + sceneName + ".unity", OpenSceneMode.Single);
        int errors = 0;

        if (Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length != 1)
        {
            errors += Fail(sceneName, "expected exactly one EventSystem");
        }

        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (canvases.Length != 1)
        {
            errors += Fail(sceneName, "expected exactly one Canvas, found " + canvases.Length);
        }
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null || scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                errors += Fail(sceneName, "Canvas '" + canvas.name + "' is not Scale With Screen Size");
            }
        }

        foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!HasLiveListener(button.onClick))
            {
                errors += Fail(sceneName, "Button '" + button.name + "' has no wired click action");
            }
        }
        foreach (Slider slider in Object.FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!HasLiveListener(slider.onValueChanged))
            {
                errors += Fail(sceneName, "Slider '" + slider.name + "' has no wired action");
            }
        }
        foreach (Toggle toggle in Object.FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!HasLiveListener(toggle.onValueChanged))
            {
                errors += Fail(sceneName, "Toggle '" + toggle.name + "' has no wired action");
            }
        }

        switch (sceneName)
        {
            case "Settings":
                errors += CheckFields<Settings>(sceneName, "volumeSlider", "sensitivitySlider", "volumeVal", "sensitivityVal", "randomSizeToggle");
                break;
            case "SelectMode":
                errors += CheckFields<ModeCardView>(sceneName, "highScoreText");
                break;
            case "SampleScene":
                errors += CheckFields<GameManager>(sceneName, "timerText", "resultScreen", "scoreResult", "accuracyResult",
                    "highScoreResult", "shotsResult", "modeResult", "newRecordBadge");
                errors += CheckFields<ScoreManager>(sceneName, "scoreText", "accuracyText");
                errors += CheckFields<PauseScript>(sceneName, "pauseMenu", "gameManager");
                errors += CheckFields<HUDController>(sceneName, "timerText", "scoreText");
                errors += CheckFields<Crosshair>(sceneName, "hitMarker");
                errors += CheckFields<UIPanel>(sceneName, "content");
                break;
        }

        if (errors == 0)
        {
            Debug.Log("[UIBuilder] " + sceneName + ": OK");
        }
        return errors;
    }

    private static bool HasLiveListener(UnityEventBase unityEvent)
    {
        int count = unityEvent.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            if (unityEvent.GetPersistentTarget(i) != null && !string.IsNullOrEmpty(unityEvent.GetPersistentMethodName(i)))
            {
                return true;
            }
        }
        return false;
    }

    private static int CheckFields<T>(string sceneName, params string[] fields) where T : Component
    {
        T[] components = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (components.Length == 0)
        {
            return Fail(sceneName, "no " + typeof(T).Name + " found");
        }

        int errors = 0;
        foreach (T component in components)
        {
            SerializedObject so = new SerializedObject(component);
            foreach (string field in fields)
            {
                SerializedProperty property = so.FindProperty(field);
                if (property == null)
                {
                    errors += Fail(sceneName, typeof(T).Name + " has no field '" + field + "'");
                }
                else if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                {
                    errors += Fail(sceneName, typeof(T).Name + "." + field + " is not assigned");
                }
            }
        }
        return errors;
    }

    private static int Fail(string sceneName, string message)
    {
        Debug.LogError("[UIBuilder] " + sceneName + ": " + message);
        return 1;
    }
}
