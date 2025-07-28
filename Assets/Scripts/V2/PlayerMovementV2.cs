using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
    public static PlayerMovementV2 Instance { get; private set; }
    public float MoveSpeed;
    public Camera FreeLookCamera;
    public float MaxRotationSpeed = 1f;
    public Vector3 _lastCameraForwardDirection;
    public Vector3 _lastCameraRightDirection;
    public Vector3 _lastCameraUpDirection;
    private bool _isFreeLooking;
    
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
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var forwardsInput = Input.GetAxisRaw("Vertical");
        var verticalInput = (Input.GetKey(KeyCode.Space) ? 1f : 0f) + (Input.GetKey(KeyCode.LeftShift) ? -1f : 0f);

        if (Input.GetMouseButtonDown(1))
        {
            _lastCameraForwardDirection = FreeLookCamera.transform.forward.normalized;
            _lastCameraRightDirection = FreeLookCamera.transform.right.normalized;
            _lastCameraUpDirection = FreeLookCamera.transform.up.normalized;
            _isFreeLooking = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isFreeLooking = false;
        }
        
        if (forwardsInput != 0f)
        {
            var forwardsSpeed = Mathf.Clamp(forwardsInput * MoveSpeed, -MoveSpeed / 2, MoveSpeed);
            MoveCharacterForward(forwardsSpeed);
        }

        if (horizontalInput != 0f)
        {
            var horizontalSpeed = (horizontalInput * MoveSpeed) / 2;
            MoveCharacterHorizontal(horizontalSpeed);
        }

        if (verticalInput != 0f)
        {
            var verticalSpeed = (verticalInput * MoveSpeed) / 2;
            MoveCharacterVertical(verticalSpeed);
        }

        if (!_isFreeLooking)
        {
            var flyDirection = FreeLookCamera.transform.forward.normalized;
            var rotateDirection = Vector3.RotateTowards(
                transform.forward, 
                flyDirection, 
                MaxRotationSpeed * Time.deltaTime,
                0.0f
            );
            transform.rotation = Quaternion.LookRotation(rotateDirection);
        }
    }

    private void MoveCharacterForward(float speed)
    {
        transform.position += (_isFreeLooking
            ? _lastCameraForwardDirection
            : FreeLookCamera.transform.forward.normalized) 
                              * (speed * Time.deltaTime);
    }

    private void MoveCharacterHorizontal(float speed)
    {
        transform.position += (_isFreeLooking
            ? _lastCameraRightDirection
            : FreeLookCamera.transform.right) 
                              * (speed * Time.deltaTime);
    }

    private void MoveCharacterVertical(float speed)
    {
        transform.position += (_isFreeLooking
        ? _lastCameraUpDirection
        : FreeLookCamera.transform.up) 
                              * (speed * Time.deltaTime);
    }
}
