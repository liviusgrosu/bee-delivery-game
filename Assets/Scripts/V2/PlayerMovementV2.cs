using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
    public static PlayerMovementV2 Instance { get; private set; }

    public float moveSpeed;

    public Camera freeLookCamera;
    private float currentHeight;
    private float xRotation;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        currentHeight = transform.position.y;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput != 0f)
        {
            var verticalSpeed = Mathf.Clamp(verticalInput * moveSpeed, -moveSpeed / 2, moveSpeed);
            MoveCharacterForward(verticalSpeed);
        }

        if (horizontalInput != 0f)
        {
            var horizontalSpeed = (horizontalInput * moveSpeed) / 2;
            MoveCharacterHorizontal(horizontalSpeed);
        }
        
        if (horizontalInput == 0f && verticalInput == 0f)
        {
            DisableMovement();
        }
        else
        {
            var flyDirection = freeLookCamera.transform.forward.normalized;
            transform.rotation = Quaternion.LookRotation(flyDirection);
        }
    }

    private void MoveCharacterForward(float speed)
    {
        transform.position += freeLookCamera.transform.forward.normalized * (speed * Time.deltaTime);
    }

    private void MoveCharacterHorizontal(float speed)
    {
        transform.position += freeLookCamera.transform.right * (speed * Time.deltaTime);
    }
    
    private void DisableMovement()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
