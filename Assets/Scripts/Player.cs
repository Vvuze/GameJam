using UnityEngine;

public class Player : MonoBehaviour
{

    // КАМера
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;
    private float horizontalInput = 0f;
    private Transform cameraTransform;

    //хождение
    private Rigidbody rb;
    public float MoveSpeed = 5f;
    private float moveHorizontal;
    private float moveForward;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveForward = Input.GetAxisRaw("Vertical");

        horizontalInput += Input.GetAxis("Mouse X") * mouseSensitivity;
        RotateCamera();
    }

    private void FixedUpdate()
    {

        if (horizontalInput != 0f)
        {
            Quaternion deltaRotation = Quaternion.Euler(0, horizontalInput, 0);
            rb.MoveRotation(rb.rotation * deltaRotation);
            horizontalInput = 0f; 
        }

        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        Vector3 targetVelocity = movement * MoveSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        if (moveHorizontal == 0 && moveForward == 0)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void RotateCamera()
    {
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
