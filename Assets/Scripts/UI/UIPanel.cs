using UnityEngine;

// Fades the panel in (and scales `content` up) whenever it is activated. Uses unscaled time so it works while paused.
[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    [SerializeField] float duration = 0.2f;
    [SerializeField] float startScale = 0.92f;
    [SerializeField] RectTransform content;

    private CanvasGroup group;
    private float t = 1f;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        t = 0f;
        Apply(0f);
    }

    void Update()
    {
        if (t >= 1f)
        {
            return;
        }
        t = Mathf.Min(1f, t + Time.unscaledDeltaTime / duration);
        Apply(1f - Mathf.Pow(1f - t, 3f));
    }

    private void Apply(float k)
    {
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
        }
        group.alpha = k;
        RectTransform target = content != null ? content : (RectTransform)transform;
        float s = Mathf.Lerp(startScale, 1f, k);
        target.localScale = new Vector3(s, s, 1f);
    }
}
