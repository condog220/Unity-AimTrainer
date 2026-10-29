using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed;
    public Transform orientation;

    public float horizontalInput;
    Vector3 moveDirection;
    Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
    }

    private void movePlayer()
    {
        moveDirection = orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

    }
}
