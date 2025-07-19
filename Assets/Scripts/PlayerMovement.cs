using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float MaxSpeed = 5f;
    public float Acceleration = 10f;
    public float Drag = 5f;
    public float SprintMultiplier = 5f;

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
        _rb = GetComponent<Rigidbody>();
        _rb.linearDamping = Drag;
    }
    
    void Update()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY =  Input.GetAxis("Mouse Y");
        _yaw += mouseX * MouseSensitivityX;
        _pitch += mouseY * MouseSensitivityY;
        
        _pitch = Mathf.Clamp(_pitch, -75, 75);
        
        transform.rotation = Quaternion.Euler(-_pitch, _yaw, 0);
        
        // Get movement input from WASD/Arrow keys
        var isSprinting = Input.GetKey(KeyCode.Space);
        var speed = isSprinting ? SprintMultiplier * MaxSpeed : MaxSpeed;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float upInput = Input.GetKey(KeyCode.Q) ? 1 : 0;
        float downInput = Input.GetKey(KeyCode.E) ? -1 : 0;
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
