using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingMovement : MonoBehaviour
{
    public static PlayerWalkingMovement Instance;
    
    [Tooltip("How fast the player will go")]
    [SerializeField]
    private float movementSpeed = 4f;
    [SerializeField]
    private Transform freeLookCamera;
    [Tooltip("Threshold angle for deciding between up and down")]
    [SerializeField]
    [Range(0, 90)]
    public float upFlipThreshold = 80.0f;
    [Tooltip("How far apart the player is from the ground after being snapped to it")]
    [SerializeField] 
    private float groundOffset = 0.03f;
    [Tooltip("Radius of collider sphere")]
    [SerializeField] 
    private float colliderCheckRadius = 0.35f;
    
    private Rigidbody _rigidbody;
    public Vector3 RelativeUp;
    private Vector3 _relativeForward, _lastKnownRelativeForward;
    private Vector3 _inputMovement;
    private int _environmentMask;
    private bool _facingUp;
    // TODO: Need to dynamically calculate it (See BUG-1)
    private bool _previousFacingUp = true;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RelativeUp = transform.up;
        _relativeForward = _lastKnownRelativeForward = transform.forward;
        _environmentMask = LayerMask.GetMask("Environment");
    }

    private void Update()
    {
        GetInputMovement();
    }

    private void FixedUpdate()
    {
        GetGroundedState();
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        _rigidbody.linearVelocity = GetDesiredMovement() * movementSpeed;
        if (Vector3.Distance(_inputMovement, Vector3.zero) > 0.001f)
        {
            _lastKnownRelativeForward = _relativeForward;
        }
        var lookRotation = Quaternion.LookRotation(_lastKnownRelativeForward, RelativeUp);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
    }
    
    private Vector3 GetDesiredMovement()
    {
        var yaw = freeLookCamera.eulerAngles.y;

        var dot = Vector3.Dot(Vector3.Project(RelativeUp, Vector3.up), Vector3.up);
        _facingUp = dot > 0 || Mathf.Approximately(dot, 0);

        if (_previousFacingUp != _facingUp)
        {
            var angle = Vector3.Angle(RelativeUp, _previousFacingUp ? Vector3.up : Vector3.down);
            if (Mathf.Abs(angle % 90) <= upFlipThreshold && !Mathf.Approximately(angle, 180))
            {
                _facingUp = _previousFacingUp;
            }
        }

        _previousFacingUp = _facingUp;
        var worldUp = _facingUp ? Vector3.up : Vector3.down;

        if (Mathf.Approximately(dot, -1))
        {
            yaw *= -1;
        }

        var planeRotation = Quaternion.FromToRotation(worldUp, RelativeUp);
        var playerRotation = Quaternion.AngleAxis(yaw, worldUp);
        _relativeForward = planeRotation * (playerRotation * Vector3.forward);
        var movementRotation = Quaternion.LookRotation(_relativeForward, RelativeUp);
        var axisMovement = movementRotation * _inputMovement;
        Debug.DrawRay(transform.position, _relativeForward, Color.blue);
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        return axisMovement * movementSpeed;
    }

    private void GetInputMovement()
    {
        var inputX = Input.GetAxis("Horizontal") * (_facingUp ? 1f : -1f);
        var inputY = Input.GetAxis("Vertical");
        _inputMovement = new Vector3(inputX, 0, inputY);
    }

    private void GetGroundedState()
    {
        // Init
        var closest = new CustomRaycastHit
        {
            Distance = Mathf.Infinity
        };
        var hitSomething = false;
        var hits = GetOverlappingHits();

        foreach (var hit in hits)
        {
            if (hit.Distance < closest.Distance)
            {
                closest = hit;
            }
            hitSomething = true;
        }

        if (!hitSomething)
        {
            return;
        }

        RelativeUp = closest.Normal;
        transform.position = closest.Point + (RelativeUp.normalized * (0.25f + groundOffset));
    }
    
    private List<CustomRaycastHit> GetOverlappingHits()
    {
        var overlappingColliders = new Collider[5];
        var hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, 
            colliderCheckRadius, 
            overlappingColliders, 
            _environmentMask);

        var rayCasts = new List<CustomRaycastHit>();

        for (var i = 0; i < hitCount; i++)
        {
            var rayCast = new CustomRaycastHit
            {
                Collider = overlappingColliders[i]
            };

            rayCast.Point = rayCast.Collider.ClosestPoint(transform.position);
            rayCast.Normal = (transform.position - rayCast.Point).normalized;
            rayCast.Distance = (transform.position - (rayCast.Point + rayCast.Normal * 0.25f)).magnitude;
            
            rayCasts.Add(rayCast);
        }
        
        return rayCasts;
    }
}
