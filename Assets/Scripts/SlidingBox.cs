using System;
using Unity.VisualScripting;
using UnityEngine;

public class SlidingBox : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private bool XLock, ZLock;
    [Tooltip("Considers where the movement is relative to where camera is facing")]
    [SerializeField] private bool isCameraRelative;
    private Rigidbody _rigidbody;

    private Camera _cam;
    
    private void Start()
    {
        _cam = Camera.main;
    }

    public void UpdateSpeed()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (isCameraRelative)
        {
            var relativeCamera = _cam.transform.TransformDirection(input);
            var projectedForce = Vector3.ProjectOnPlane(relativeCamera, transform.up).normalized;
            input = transform.InverseTransformDirection(projectedForce);
        }
    
        if (XLock)
        {
            input.x = 0f;
        }
        if (ZLock)
        {
            input.z = 0f;
        }
    
        var lockedWorldForce = transform.TransformDirection(input) * (_speed * Time.deltaTime);
        
        _rigidbody.AddForce(lockedWorldForce, ForceMode.Force);
    }
    
    public void StartGrip()
    {
        ToggleRigidbody(true);
    }

    public void EndGrip()
    {
        ToggleRigidbody(false);
        _rigidbody.linearVelocity = Vector3.zero;
    }

    private void ToggleRigidbody(bool state)
    {
        if (state && !_rigidbody)
        {
            _rigidbody = this.AddComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = false;
            _rigidbody.freezeRotation = true;
            _rigidbody.mass = 0.01f;
            _rigidbody.linearDamping = 10f;
            // TODO: freeze positions maybe?
        }
        else
        {
            Destroy(_rigidbody);
        }
    }
}
