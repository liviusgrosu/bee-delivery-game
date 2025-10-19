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
        
        HandleFreeLooking();
        HandleRotation();

        ApplyBuzzing();
    }

    private void FixedUpdate()
    {
        HandleFlyingMovement();
        ClampVelocity();
    }

    private void HandleFlyingMovement()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var forwardsInput = Input.GetAxisRaw("Vertical");
        var verticalInput = (Input.GetKey(KeyCode.Space) ? 1f : 0f) 
                                + (Input.GetKey(KeyCode.LeftShift) ? -1f : 0f);
        
        if (forwardsInput != 0f)
        {
            AddForce(_isFreeLooking 
                        ? _lastCameraForwardDirection 
                        : FreeLookCamera.transform.forward,
                        forwardsInput);
        }

        if (horizontalInput != 0f)
        {
            AddForce(_isFreeLooking 
                    ? _lastCameraRightDirection 
                    : FreeLookCamera.transform.right,
                horizontalInput * 0.5f);
        }

        if (verticalInput != 0f)
        {
            AddForce(_isFreeLooking
                    ? _lastCameraUpDirection
                    : FreeLookCamera.transform.up,
                verticalInput * 0.5f);
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
