using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class WaspBehaviour : MonoBehaviour
{
    enum State
    {
        Idle,
        Chasing,
        Patroling,
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

    private bool _isLockedFromAttacking;
    [SerializeField]
    private float _lockedTime;

    public EnemyPathing path;
    private Transform _currentPoint;
    private int _pathIndex;
    private bool _isPatroling => path != null;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _currentState = _isPatroling ? State.Patroling : State.Idle;
    }

    private void Start()
    {
        if (_isPatroling)
        {
            _currentPoint = path.Points[_pathIndex];
        }
        
        _originalPosition =  transform.position;
        _originalRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (_currentState == State.Chasing)
        {
            var direction = _player.position - transform.position;
            var velocityChange = ((direction * MaxSpeed) - _rb.linearVelocity);
            
            if (!_isLockedFromAttacking)
            {
                _rb.AddForce(velocityChange * Acceleration, ForceMode.Acceleration);
            }

            transform.rotation = Quaternion.LookRotation(direction);
        }
        else if (_currentState == State.Returning)
        {
            var distanceFromSpawn = Vector3.Distance(transform.position, _originalPosition);

            if (distanceFromSpawn < 0.1f)
            {
                _rb.linearVelocity = Vector3.zero;
                transform.rotation = _originalRotation;
                transform.position = _originalPosition;
                _currentState = _isPatroling ? State.Patroling : State.Idle;
                return;
            }
            
            var direction = _originalPosition - transform.position;
            var velocityChange = ((direction * MaxSpeed) - _rb.linearVelocity);
            _rb.AddForce(velocityChange * Acceleration, ForceMode.Acceleration);
            transform.rotation = Quaternion.LookRotation(_originalPosition);
        }
        else if (_currentState == State.Patroling)
        {
            var distanceFromPoint = Vector3.Distance(transform.position, _currentPoint.position);
            
            if (distanceFromPoint < 0.1f)
            {
                _pathIndex = _pathIndex >= path.Points.Count - 1 
                    ? 0
                    : _pathIndex + 1;
                
                _currentPoint = path.Points[_pathIndex];
                return;
            }
            
            var direction = _currentPoint.position - transform.position;
            var velocityChange = ((direction * MaxSpeed) - _rb.linearVelocity);
            _rb.AddForce(velocityChange * Acceleration, ForceMode.Acceleration);
            transform.rotation = Quaternion.LookRotation(_currentPoint.position);
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

    private void OnCollisionEnter(Collision other)
    {
        var obj = other.gameObject;

        if (obj.CompareTag("Player") && !_isLockedFromAttacking)
        {
            PlayerSoundPlayer.Instance.PlayHurtSound();
            var pickUp = obj.GetComponentInChildren<PickUp>();
            if (pickUp.PickedUpItem)
            {
                pickUp.DropPackage();
            }
            
            StartCoroutine(PauseEnemy());
        }
    }

    private IEnumerator PauseEnemy()
    {
        _isLockedFromAttacking = true;
        _rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(_lockedTime);
        _isLockedFromAttacking = false;
    }
}
