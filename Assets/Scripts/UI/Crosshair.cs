using UnityEngine;

// Hit marker that flashes around the crosshair on a hit and fades out.
public class Crosshair : MonoBehaviour
{
    public static Crosshair instance;

    [SerializeField] CanvasGroup hitMarker;
    [SerializeField] float fadeSpeed = 6f;

    void Awake()
    {
        instance = this;
        if (hitMarker != null)
        {
            hitMarker.alpha = 0f;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void Flash()
    {
        if (hitMarker != null)
        {
            hitMarker.alpha = 1f;
        }
    }

    void Update()
    {
        if (hitMarker != null && hitMarker.alpha > 0f)
        {
            hitMarker.alpha = Mathf.Max(0f, hitMarker.alpha - fadeSpeed * Time.unscaledDeltaTime);
        }
    }
}
