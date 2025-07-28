using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
    public static PlayerMovementV2 Instance { get; private set; }
    public float MoveSpeed;
    public Camera FreeLookCamera;
    public float MaxRotationSpeed = 1f;
    public Vector3 LastCameraForwardDirection;
    public Vector3 LastCameraRightDirection;
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
            LastCameraForwardDirection = FreeLookCamera.transform.forward.normalized;
            LastCameraRightDirection = FreeLookCamera.transform.right.normalized;
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

        if (!Input.GetMouseButton(1))
        {
            var flyDirection = FreeLookCamera.transform.forward.normalized;
            var rotateDirection = Vector3.RotateTowards(
                transform.forward, 
                flyDirection, 
                MaxRotationSpeed * Time.deltaTime,
                0.0f
            );
            transform.rotation = Quaternion.LookRotation(
                rotateDirection,
                FreeLookCamera.transform.up
            );
        }
    }

    private void MoveCharacterForward(float speed)
    {
        transform.position += FreeLookCamera.transform.forward.normalized * (speed * Time.deltaTime);
    }

    private void MoveCharacterHorizontal(float speed)
    {
        transform.position += FreeLookCamera.transform.right * (speed * Time.deltaTime);
    }

    private void MoveCharacterVertical(float speed)
    {
        transform.position += FreeLookCamera.transform.up * (speed * Time.deltaTime);
    }
}
