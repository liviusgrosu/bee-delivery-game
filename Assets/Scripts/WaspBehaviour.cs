using System;
using UnityEngine;

public class WaspBehaviour : MonoBehaviour
{
    enum State
    {
        Idle,
        Chasing,
        Returning
    }

    [Header("Movement")]
    public float MaxSpeed = 5f;
    public float Acceleration = 10f;
    
    private Rigidbody _rb;
    private Vector3 _direction;
    [SerializeField]
    private State _currentState;
    private Transform _player;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _currentState = State.Idle;
    }

    private void Start()
    {
        _originalPosition =  transform.position;
        _originalRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (_currentState == State.Chasing)
        {
            var direction = _player.position - transform.position;
            var velocityChange = ((direction * MaxSpeed) - _rb.linearVelocity);
            _rb.AddForce(velocityChange * Acceleration, ForceMode.Acceleration);
        }
        else if (_currentState == State.Returning)
        {
            var distanceFromSpawn = Vector3.Distance(transform.position, _originalPosition);

            if (distanceFromSpawn < 0.1f)
            {
                _rb.linearVelocity = Vector3.zero;
                transform.rotation = _originalRotation;
                transform.position = _originalPosition;
                _currentState = State.Idle;
                return;
            }
            
            var direction = _originalPosition - transform.position;
            var velocityChange = ((direction * MaxSpeed) - _rb.linearVelocity);
            _rb.AddForce(velocityChange * Acceleration, ForceMode.Acceleration);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _currentState != State.Chasing)
        {
            _player = other.transform;
            _currentState = State.Chasing;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _currentState == State.Chasing)
        {
            _player = null;
            _currentState = State.Returning;
        }
    }
}
