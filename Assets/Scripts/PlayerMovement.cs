using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed;
    public Transform orientation;

    public float horizontalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    Transform cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput();

    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    private void moveInput()
    {
        if(GameModeSelect.currentMode?.UsesMovement == true)
        {
            // Strafe relative to where the player is facing (yaw only), not world X.
            orientation.rotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
    }

    private void movePlayer()
    {
        moveDirection = orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

    }
}
