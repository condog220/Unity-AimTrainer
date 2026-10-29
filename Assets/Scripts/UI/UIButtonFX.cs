using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Hover / press scale animation and click sounds. Uses unscaled time so it works while the game is paused.
public class UIButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float hoverScale = 1.06f;
    [SerializeField] float pressScale = 0.96f;
    [SerializeField] float speed = 14f;
    [SerializeField] bool playSounds = true;

    private float target = 1f;
    private bool hovered;
    private bool pressed;
    private Selectable selectable;

    void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    void OnEnable()
    {
        hovered = false;
        pressed = false;
        target = 1f;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        float s = transform.localScale.x;
        if (Mathf.Abs(s - target) < 0.0005f)
        {
            return;
        }
        s = Mathf.Lerp(s, target, 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime));
        transform.localScale = new Vector3(s, s, 1f);
    }

    private bool Interactable => selectable == null || selectable.IsInteractable();

    private void Refresh()
    {
        target = pressed ? pressScale : (hovered ? hoverScale : 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;
        hovered = true;
        if (playSounds) UIAudio.PlayHover();
        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        pressed = false;
        Refresh();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;
        pressed = true;
        if (playSounds) UIAudio.PlayClick();
        Refresh();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Refresh();
    }
}
