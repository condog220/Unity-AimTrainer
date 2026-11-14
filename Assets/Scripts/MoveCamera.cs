using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        float rotationX = 0f;
        float rotationY = 0f;
        public float userSens;
        [SerializeField] private float sensMultiplier;
    [SerializeField] private Transform player;
    public Transform camPos;
    public Transform gunPosition;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        SettingsManager.Changed += ApplySettings;
    }

    // Static event: unsubscribe, or it would keep calling this camera after the scene is gone.
    void OnDisable()
    {
        SettingsManager.Changed -= ApplySettings;
    }

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        ApplySettings();
    }

    // Also runs when a setting is changed from the pause menu.
    private void ApplySettings()
    {
        userSens = SettingsManager.sensitivity;
        cam.fieldOfView = SettingsManager.fov;
    }

    // LateUpdate so the camera follows the Rigidbody player after physics has moved it.
    void LateUpdate()
    {
        if (GameManager.InputLocked)
        {
            return;
        }

        rotationY += Input.GetAxisRaw("Mouse X") * userSens;
        rotationX -=  Input.GetAxisRaw("Mouse Y") * userSens;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        transform.position = player.position;
    }
}
