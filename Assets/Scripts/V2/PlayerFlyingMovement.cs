using System;
using UnityEngine;

public class PlayerFlyingMovement : MonoBehaviour
{
    public static PlayerFlyingMovement Instance { get; private set; }
    
    // Camera
    [SerializeField] private Camera freeLookCamera;
    private Vector3 _lastCameraForwardDirection;
    private Vector3 _lastCameraRightDirection;
    private Vector3 _lastCameraUpDirection;

    private bool _isFreeLooking;

    [Header("Movement")] 
    [SerializeField] private float maxSpeed;
    [SerializeField] private float forwardAcceleration = 15f;
    [SerializeField] private float backwardAcceleration = 8f;
    [SerializeField] private float strafeAcceleration = 10f;
    [SerializeField] private float verticalAcceleration = 10f;
    [SerializeField] private float maxRotationSpeed = 1f;
    [SerializeField] private float drag = 2f;
    
    private Vector3 _inputDirection;
    
    [Header("Bobbing Effect")]
    [SerializeField] private float bobAmplitude = 0.05f; 
    [SerializeField] private float bobFrequency = 5f;    
    [SerializeField] private float swayAmplitude = 0.05f;
    [SerializeField] private float swayFrequency = 3f;   
    [SerializeField] private float transitionSpeed = 5f; 
    
    private float _currentBobAmplitude;
    private float _currentBobFrequency;
    private float _currentSwayAmplitude;
    private float _currentSwayFrequency;
    
    // Misc.    
    [SerializeField] private Transform model;
    private Vector3 _initialLocalPos;
    private float _currentSwayTime;
    private Rigidbody _rigidbody;
    
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
        _rigidbody.linearDamping = 0f;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (model != null)
        {
            _initialLocalPos = model.localPosition;
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
        HandleRotation();
        HandleFreeLooking();
        ApplyBuzzing();
    }

    private void FixedUpdate()
    {
        HandleAdvancedMovement();
        ApplyDrag();
        ClampVelocity();
    }
    
    void GetInput()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var forwards = Input.GetAxisRaw("Vertical");
        var vertical = (Input.GetKey(KeyCode.Space) ? 1f : 0f) 
                            + (Input.GetKey(KeyCode.LeftShift) ? -1f : 0f);
        
        _inputDirection = new Vector3(horizontal, vertical, forwards);
    }
    
    private void HandleAdvancedMovement()
    {
        // TODO: we might want to convert our _inputDirection to local space first
        if (!(_inputDirection.magnitude > 0.1f))
        {
            return;
        }
        var force = CalculateAccelerationForce(_inputDirection);
        var relativeForce = GetRelativeForceDirection(force);
        _rigidbody.AddForce(relativeForce, ForceMode.Acceleration);
    }

    private Vector3 GetRelativeForceDirection(Vector3 force)
    {
        if (_isFreeLooking)
        {
            return force.x * _lastCameraRightDirection+
                   force.y * _lastCameraUpDirection +
                   force.z * _lastCameraForwardDirection;
        }

        return freeLookCamera.transform.TransformDirection(force);
    }
    
    private Vector3 CalculateAccelerationForce(Vector3 direction)
    {
        var accelerationForce = Vector3.zero;

        accelerationForce.z = direction.z switch
        {
            > 0 => direction.z * forwardAcceleration,
            < 0 => direction.z * backwardAcceleration,
            _ => accelerationForce.z
        };
        
        accelerationForce.x = direction.x * strafeAcceleration;
        accelerationForce.y = direction.y * verticalAcceleration;
        return transform.TransformDirection(accelerationForce);
    }
    
    private void ApplyDrag()
    {
        if (_inputDirection.magnitude < 0.1f && _rigidbody.linearVelocity.magnitude > 0.1f)
        {
            _rigidbody.linearVelocity *= (1f - drag * Time.fixedDeltaTime);
        }
    }
    
    private void ClampVelocity()
    {
        // Separate clamping for horizontal and vertical if needed
        var horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, _rigidbody.linearVelocity.y, horizontalVelocity.z);
        }
    }
    
    private void HandleFreeLooking()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _lastCameraForwardDirection = freeLookCamera.transform.forward.normalized;
            _lastCameraRightDirection = freeLookCamera.transform.right.normalized;
            _lastCameraUpDirection = freeLookCamera.transform.up.normalized;
            _isFreeLooking = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isFreeLooking = false;
        }
    }
    
    private void HandleRotation()
    {
        if (_isFreeLooking)
        {
            return;
        }
        var flyDirection = freeLookCamera.transform.forward.normalized;
        var rotateDirection = Vector3.RotateTowards(
            model.forward, 
            flyDirection, 
            maxRotationSpeed * Time.deltaTime,
            0.0f
        );
        model.rotation = Quaternion.LookRotation(rotateDirection);
    }
    
    public void ResetModelRotation()
    {
        model.rotation = Quaternion.LookRotation(transform.forward, transform.up);
    }

    private void ApplyBuzzing()
    {
        if (!model)
        {
            return;
        }

        _currentBobAmplitude = Mathf.Lerp(_currentBobAmplitude, bobAmplitude, Time.deltaTime * transitionSpeed);
        _currentBobFrequency = Mathf.Lerp(_currentBobFrequency, bobFrequency, Time.deltaTime * transitionSpeed);

        _currentSwayAmplitude = Mathf.Lerp(_currentSwayAmplitude, swayAmplitude, Time.deltaTime * transitionSpeed);
        _currentSwayFrequency = Mathf.Lerp(_currentSwayFrequency, swayFrequency, Time.deltaTime * transitionSpeed);

        var time = Time.time;
        var bobOffset = Mathf.Sin(time * _currentBobFrequency) * _currentBobAmplitude;
        var swayOffset = Mathf.Cos(time * _currentSwayFrequency) * _currentSwayAmplitude;
        
        model.localPosition = _initialLocalPos + new Vector3(swayOffset, bobOffset, 0f);
    }
}
