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

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        userSens = SettingsManager.sensitivity;
    }

    // LateUpdate so the camera follows the Rigidbody player after physics has moved it.
    void LateUpdate()
    {
        if (PauseScript.isPaused)
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
