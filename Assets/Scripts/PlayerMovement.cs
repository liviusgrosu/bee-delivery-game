using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]
    public float MaxSpeed = 5f;
    public float Acceleration = 10f;
    public float Drag = 5f;
    public float SprintMultiplier = 5f;

    [HideInInspector]
    public bool IsSprinting;
    private float _currentSpeed;
    
    [Header("Mouse")]
    public float MouseSensitivityX = 5f;
    public float MouseSensitivityY = 5f;
    
    private Rigidbody _rb;
    private Vector3 _input;
    private float _yaw;
    private float _pitch;

    public PickUp PickUp;
    public float CarryCapacity = 2f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _rb = GetComponent<Rigidbody>();
        _rb.linearDamping = Drag;
    }
    
    void Update()
    {
        if (UIManager.Instance.CurrentMenu == UIManager.MenuState.JobList)
        {
            return;
        }
        
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY =  Input.GetAxis("Mouse Y");
        _yaw += mouseX * MouseSensitivityX;
        _pitch += mouseY * MouseSensitivityY;
        
        _pitch = Mathf.Clamp(_pitch, -75, 75);
        
        transform.rotation = Quaternion.Euler(-_pitch, _yaw, 0);
        
        // Get movement input from WASD/Arrow keys
        IsSprinting = Input.GetKey(KeyCode.Mouse0);
        var speed = IsSprinting ? SprintMultiplier * MaxSpeed : MaxSpeed;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float upInput = Input.GetKey(KeyCode.Space) ? 1 : 0;
        float downInput = Input.GetKey(KeyCode.LeftShift) ? -1 : 0;
        var combinedUpDown = upInput + downInput;

        var weightForce = Mathf.Clamp(PickUp.CurrentWeight / CarryCapacity, 1f, Mathf.Infinity);
        speed /= weightForce;
        
        var moveInput = new Vector3(horizontal * speed, combinedUpDown * speed, vertical * speed);
        _input = transform.TransformDirection(moveInput);
    }
    
    void FixedUpdate()
    {
        if (_input.magnitude > 0.1f)
        {
            Vector3 velocityChange = (_input - _rb.linearVelocity);
            _rb.AddForce(velocityChange * Acceleration, ForceMode.Acceleration);
        }
    }
}
