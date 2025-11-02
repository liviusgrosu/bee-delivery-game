using System;
using System.Collections;
using System.IO.IsolatedStorage;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerFlyingMovement : MonoBehaviour
{
    public static PlayerFlyingMovement Instance { get; private set; }
    
    // Camera
    [SerializeField] private Camera freeLookCamera;
    private Vector3 _lastCameraForwardDirection;
    private Vector3 _lastCameraRightDirection;
    private Vector3 _lastCameraUpDirection;

    public bool IsFreeLooking;

    [Header("Movement")] 
    [SerializeField] private float maxHorizontalSpeed = 10f;
    [SerializeField] private float maxVerticalSpeed = 5f;

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
    
    [Header("Stun")]
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private float stunSpeedThreshold = 6f;
    private float _currentStunTime;
    private Vector3 _stunDirection;
    private Vector3 _previousVelocity;
    private bool _isStunned;

    public bool IsStunned
    {
        get => _isStunned;
        set
        {
            _isStunned = value;
            StunnedEvent?.Invoke(_isStunned);
        }
    }
    
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
    public static event Action<Vector3> RigidBodyVelocityChange;
    public static event Action<bool> StunnedEvent;
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
        if (Input.GetKey(KeyCode.T))
        {
            Debug.Break();
        }

        // Store previous velocity as this happens after OnCollisionEnter
        _previousVelocity = _rigidbody.linearVelocity;
        
        if (!IsStunned)
        {
            GetInput();
            HandleRotation();
            HandleFreeLooking();
        }
        ApplyBuzzing();
    }

    private void FixedUpdate()
    {
        if (!IsStunned)
        {
            HandleAdvancedMovement();
            ApplyDrag();
            ClampVelocity();
        }
        DebugOutputSpeed();
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
        if (IsFreeLooking)
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
        var horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        var verticalVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
    
        // Clamp horizontal speed
        if (horizontalVelocity.magnitude > maxHorizontalSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;
        }
    
        // Clamp vertical speed separately
        verticalVelocity = Vector3.ClampMagnitude(verticalVelocity, maxVerticalSpeed);
        _rigidbody.linearVelocity = horizontalVelocity + verticalVelocity;
    }
    
    private void HandleFreeLooking()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _lastCameraForwardDirection = freeLookCamera.transform.forward.normalized;
            _lastCameraRightDirection = freeLookCamera.transform.right.normalized;
            _lastCameraUpDirection = freeLookCamera.transform.up.normalized;
            IsFreeLooking = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            IsFreeLooking = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!(_previousVelocity.magnitude >= stunSpeedThreshold)) return;
        if (Physics.Raycast(transform.position, _previousVelocity, out var hit, 1f,
                LayerMask.GetMask("Environment")))
        {
            var normal = collision.contacts[0].normal;
            _stunDirection = Vector3.Reflect(_previousVelocity, normal);
        }
        IsStunned = true;
        IsFreeLooking = false;
        _rigidbody.linearVelocity = Vector3.zero;
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        var startPos = transform.position;
        var endPos = startPos + _stunDirection.normalized;
        var cTime = 0f;

        
        while (cTime < stunDuration)
        {
            cTime += Time.deltaTime;
            var step = cTime / stunDuration;
            transform.position = Vector3.Lerp(startPos, endPos, step); 
            yield return null;
        }

        IsStunned = false;
    }
    #region Rotation
    
    private void HandleRotation()
    {
        if (IsFreeLooking)
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
    
    #endregion

    #region Effects
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
    #endregion

    #region Debug

    private void DebugOutputSpeed()
    {
        RigidBodyVelocityChange?.Invoke(_rigidbody.linearVelocity);
    }

    #endregion
}
