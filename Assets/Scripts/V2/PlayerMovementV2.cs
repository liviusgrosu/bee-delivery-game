using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
    public static PlayerMovementV2 Instance { get; private set; }
    public float MoveSpeed;
    public Camera FreeLookCamera;
    public float MaxRotationSpeed = 1f;
    private Vector3 _lastCameraForwardDirection;
    private Vector3 _lastCameraRightDirection;
    private Vector3 _lastCameraUpDirection;
    private bool _isFreeLooking;
    
    public Transform Model;
    
    [Header("Bobbing Effect")]
    [SerializeField] private float _bobAmplitude = 0.05f; 
    [SerializeField] private float _bobFrequency = 5f;    

    [SerializeField] private float _swayAmplitude = 0.05f;
    [SerializeField] private float _swayFrequency = 3f;   
    
    private float _currentBobAmplitude;
    private float _currentBobFrequency;
    private float _currentSwayAmplitude;
    private float _currentSwayFrequency;
    
    [SerializeField] private float transitionSpeed = 5f; 
    
    private Vector3 _initialLocalPos;
    private bool _isMoving = false;
    private float _currentSwayTime;

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

        if (Model != null)
        {
            _initialLocalPos = Model.localPosition;
        }
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
            MoveCharacterForward(forwardsInput);
        }

        if (horizontalInput != 0f)
        {
            MoveCharacterHorizontal(horizontalInput);
        }

        if (verticalInput != 0f)
        {
            MoveCharacterVertical(verticalInput);
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

        if (horizontalInput == 0f && verticalInput == 0f && forwardsInput == 0f)
        {
            _isMoving = false;
        }
        
        ApplyBuzzing();
    }

    private void MoveCharacterForward(float forwardsInput)
    {
        _isMoving = true;
        var speed = Mathf.Clamp(forwardsInput * MoveSpeed, -MoveSpeed / 2, MoveSpeed);
        transform.position += (_isFreeLooking
            ? _lastCameraForwardDirection
            : FreeLookCamera.transform.forward.normalized) 
                              * (speed * Time.deltaTime);
    }

    private void MoveCharacterHorizontal(float horizontalInput)
    {
        _isMoving = true;
        var speed = (horizontalInput * MoveSpeed) / 2;
        transform.position += (_isFreeLooking
            ? _lastCameraRightDirection
            : FreeLookCamera.transform.right) 
                              * (speed * Time.deltaTime);
    }

    private void MoveCharacterVertical(float verticalInput)
    {
        _isMoving = true;
        var speed = (verticalInput * MoveSpeed) / 2;
        transform.position += (_isFreeLooking
            ? _lastCameraUpDirection
            : FreeLookCamera.transform.up) 
                              * (speed * Time.deltaTime);
    }

    private void ApplyBuzzing()
    {
        if (!Model)
        {
            return;
        }

        _currentBobAmplitude = Mathf.Lerp(_currentBobAmplitude, _bobAmplitude, Time.deltaTime * transitionSpeed);
        _currentBobFrequency = Mathf.Lerp(_currentBobFrequency, _bobFrequency, Time.deltaTime * transitionSpeed);

        _currentSwayAmplitude = Mathf.Lerp(_currentSwayAmplitude, _swayAmplitude, Time.deltaTime * transitionSpeed);
        _currentSwayFrequency = Mathf.Lerp(_currentSwayFrequency, _swayFrequency, Time.deltaTime * transitionSpeed);

        var time = Time.time;
        var bobOffset = Mathf.Sin(time * _currentBobFrequency) * _currentBobAmplitude;
        var swayOffset = Mathf.Cos(time * _currentSwayFrequency) * _currentSwayAmplitude;
        
        Model.localPosition = _initialLocalPos + new Vector3(swayOffset, bobOffset, 0f);
    }
}
