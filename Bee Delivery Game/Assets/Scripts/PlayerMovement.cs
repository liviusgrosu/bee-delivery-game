using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 3f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public bool invertY = false;

    private float pitch = 0f; // X-axis rotation
    private float yaw = 0f;   // Y-axis rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Hide and lock cursor
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1 : -1);

        yaw += mouseX;
        pitch += mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 localMovement = new Vector3(h, 0, v);
        Vector3 move = transform.TransformDirection(localMovement) * (moveSpeed * Time.deltaTime);

        // Vertical movement (Y axis)
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up * (verticalSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift))
            move += Vector3.down * (verticalSpeed * Time.deltaTime);

        transform.position += move;
    }
}
