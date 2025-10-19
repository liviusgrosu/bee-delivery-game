using System;
using UnityEngine;

public class PlayerFlyingMovement : MonoBehaviour
{
    public static PlayerFlyingMovement Instance { get; private set; }
    public Camera FreeLookCamera;
    public float MaxRotationSpeed = 1f;
    private Vector3 _lastCameraForwardDirection;
    private Vector3 _lastCameraRightDirection;
    private Vector3 _lastCameraUpDirection;
    
    private float _horizontalInput;
    private float _forwardsInput;
    private float _verticalInput;

    private float HorizontalInput
    {
        get => _horizontalInput;
        set
        {
            _horizontalInput = value;
            OnHorizontalInputChange?.Invoke(_horizontalInput);
        }
    }
    
    private float ForwardsInput
    {
        get => _forwardsInput;
        set
        {
            _forwardsInput = value;
            OnHorizontalInputChange?.Invoke(_forwardsInput);
        }
    }
    
    private float VerticalInput
    {
        get => _verticalInput;
        set
        {
            _verticalInput = value;
            OnHorizontalInputChange?.Invoke(_verticalInput);
        }
    }
    
    [SerializeField] private float _movementAcceleration = 0.5f;
    
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
    private float _currentSwayTime;

    private Rigidbody _rigidbody;
    [SerializeField] private float _maxSpeed;
    
    private bool _isFreeLooking;
    
    // Events
    public static event Action<float> OnHorizontalInputChange;
    public static event Action<float> OnForwardsInputChange;
    public static event Action<float> OnVerticalInputChange;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.linearDamping = 2f;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (Model != null)
        {
            _initialLocalPos = Model.localPosition;
        }
    }
    
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        if (Input.GetKey(KeyCode.T))
        {
            Debug.Break();
        }

        GetInput();
        HandleFreeLooking();
        HandleRotation();

        ApplyBuzzing();
    }

    private void FixedUpdate()
    {
        HandleFlyingMovement();
        ClampVelocity();
    }

    private void GetInput()
    {
        // REWORK
        if (Input.GetAxis("Horizontal") != 0f)
        {
            HorizontalInput += Input.GetAxisRaw("Horizontal") * _movementAcceleration * Time.deltaTime;
            HorizontalInput = Mathf.Clamp(HorizontalInput, -1f, 1f);
        }
        else
        {
            HorizontalInput = 0f;
        }
    }
    
    private void HandleFlyingMovement()
    {
        if (HorizontalInput != 0f)
        {
            AddForce(_isFreeLooking 
                    ? _lastCameraRightDirection 
                    : FreeLookCamera.transform.right,
                HorizontalInput * 0.5f);
        }
    }
    
    private void HandleRotation()
    {
        if (_isFreeLooking)
        {
            return;
        }
        var flyDirection = FreeLookCamera.transform.forward.normalized;
        var rotateDirection = Vector3.RotateTowards(
            Model.forward, 
            flyDirection, 
            MaxRotationSpeed * Time.deltaTime,
            0.0f
        );
        Model.rotation = Quaternion.LookRotation(rotateDirection);
    }

    public void ResetModelRotation()
    {
        Model.rotation = Quaternion.LookRotation(transform.forward, transform.up);
    }
    
    private void HandleFreeLooking()
    {
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
    }

    private void AddForce(Vector3 direction, float input)
    {
        _rigidbody.AddForce(direction.normalized * (input * _maxSpeed), ForceMode.Force);
    }

    private void ClampVelocity()
    {
        if (_rigidbody.linearVelocity.magnitude > _maxSpeed)
        {
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _maxSpeed;
        }
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
