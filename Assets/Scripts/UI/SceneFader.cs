using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Black fade between scenes. Creates itself on first use, so no scene object is needed.
public class SceneFader : MonoBehaviour
{
    private const float Duration = 0.25f;

    private static SceneFader instance;
    private CanvasGroup group;
    private bool loading;

    private static SceneFader Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SceneFader", typeof(RectTransform));
                DontDestroyOnLoad(go);
                instance = go.AddComponent<SceneFader>();
                instance.Build();
            }
            return instance;
        }
    }

    // Fade in from black when the game starts.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneFader fader = Instance;
        fader.group.alpha = 1f;
        fader.StartCoroutine(fader.Fade(0f));
    }

    public static void Load(string sceneName)
    {
        SceneFader fader = Instance;
        if (fader.loading)
        {
            return;
        }
        fader.StartCoroutine(fader.LoadRoutine(sceneName));
    }

    private void Build()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        GameObject fade = new GameObject("Fade", typeof(RectTransform));
        fade.transform.SetParent(transform, false);
        RectTransform rt = (RectTransform)fade.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        UnityEngine.UI.Image image = fade.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        group = gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (group == null)
        {
            return;
        }
        loading = false;
        StartCoroutine(Fade(0f));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        loading = true;
        yield return Fade(1f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Fade(float to)
    {
        float from = group.alpha;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / Duration)
        {
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }
}
