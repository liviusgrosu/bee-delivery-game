using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
    
    [HideInInspector]
    public bool IsFreeLooking;

    private bool _isCarryingLargePackage;

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
    [HideInInspector]
    public Rigidbody Rigidbody;
    
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
        
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.linearDamping = 0f;
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
        _previousVelocity = Rigidbody.linearVelocity;
        
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

        if (_inputDirection != Vector3.zero)
        {
            relativeForce.y += _inputDirection.y * verticalAcceleration;
        }
        
        Rigidbody.AddForce(relativeForce, ForceMode.Acceleration);
    }

    private Vector3 GetRelativeForceDirection(Vector3 force)
    {
        if (IsFreeLooking)
        {
            return force.x * _lastCameraRightDirection+
                   force.y * _lastCameraUpDirection +
                   force.z * _lastCameraForwardDirection;
        }
        
        var freeLockCamRelativeAxis = freeLookCamera.transform.TransformDirection(force);
        
        //Debug.DrawRay(transform.position, freeLockCamRelativeAxis, Color.red);

        return freeLockCamRelativeAxis;
    }
    
    private Vector3 CalculateAccelerationForce(Vector3 direction)
    {
        // Each axis has its own acceleration force
        var accelerationForce = Vector3.zero;
        accelerationForce.z = direction.z switch
        {
            > 0 => direction.z * forwardAcceleration,
            < 0 => direction.z * backwardAcceleration,
            _ => accelerationForce.z
        };
        accelerationForce.x = direction.x * strafeAcceleration;
        //accelerationForce.y = direction.y * verticalAcceleration;
        // Translate this from world space to the transform local space 
        return transform.TransformDirection(accelerationForce);
    }
    
    private void ApplyDrag()
    {
        if (_inputDirection.magnitude < 0.1f && Rigidbody.linearVelocity.magnitude > 0.1f)
        {
            Rigidbody.linearVelocity *= (1f - drag * Time.fixedDeltaTime);
        }
    }
    
    private void ClampVelocity()
    {
        var horizontalVelocity = new Vector3(Rigidbody.linearVelocity.x, 0, Rigidbody.linearVelocity.z);
        var verticalVelocity = new Vector3(0, Rigidbody.linearVelocity.y, 0);
    
        // Clamp horizontal speed
        if (horizontalVelocity.magnitude > WeightAffectedValue(maxHorizontalSpeed))
        {
            horizontalVelocity = horizontalVelocity.normalized * WeightAffectedValue(maxHorizontalSpeed);
        }
    
        // Clamp vertical speed separately
        verticalVelocity = Vector3.ClampMagnitude(verticalVelocity, WeightAffectedValue(maxVerticalSpeed));
        Rigidbody.linearVelocity = horizontalVelocity + verticalVelocity;
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

    private void OnCollisionEnter(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (contact.thisCollider.CompareTag("Player"))
            {
                if (!(_previousVelocity.magnitude >= stunSpeedThreshold) /*||
    other.gameObject.layer != LayerMask.NameToLayer("Environment")*/)
                {
                    return;
                }
                var normal = other.contacts[0].normal;
                _stunDirection = Vector3.Reflect(_previousVelocity, normal);
                IsStunned = true;
                IsFreeLooking = false;
                Rigidbody.linearVelocity = Vector3.zero;
                StartCoroutine(StunCoroutine());
            }
            else if (contact.thisCollider.CompareTag("Package"))
            {
                PackagePickupController.Instance.CurrentPackageComp.TakeDamage(
                    _previousVelocity.magnitude, other.contacts);
            }
        }
    }

    private float WeightAffectedValue(float maxSpeed)
    {
        // Affect the value based on the weight of the package
        return maxSpeed * PackagePickupController.Instance.GetCarryingWeightPerc();
    }
    
    #region Rotation
    
    private void HandleRotation()
    {
        if (IsFreeLooking)
        {
            return;
        }
        var flyDirection = freeLookCamera.transform.forward.normalized;
        
        if (PackagePickupController.Instance.IsHoldingLargePackage() ||
            PackagePickupController.Instance.IsHoldingLargeObject())
        {
            flyDirection = Vector3.ProjectOnPlane(flyDirection, Vector3.up);
        }
        
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
        var bobOffset = Mathf.Sin(time * WeightAffectedValue(_currentBobFrequency)) * WeightAffectedValue(_currentBobAmplitude);
        var swayOffset = Mathf.Cos(time * WeightAffectedValue(_currentSwayFrequency)) * WeightAffectedValue(_currentSwayAmplitude);
        
        model.localPosition = _initialLocalPos + new Vector3(swayOffset, bobOffset, 0f);
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
    #endregion

    #region Debug

    private void DebugOutputSpeed()
    {
        RigidBodyVelocityChange?.Invoke(Rigidbody.linearVelocity);
    }

    #endregion
}
