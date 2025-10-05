using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRaycastHits
{
    public Collider Collider { get; set; }
    public Vector3 Point { get; set; }
    public Vector3 Normal { get; set; }
    public float Distance { get; set; }
}

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
    private Vector3 _relativeUp;
    private Vector3 _inputMovement;
    private int _environmentMask;
    private bool _facingUp, _previousFacingUp;
    
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
        _relativeUp = transform.up;
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
        var rotation = Quaternion.FromToRotation(Vector3.up, _relativeUp);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.deltaTime);
    }
    
    private Vector3 GetDesiredMovement()
    {
        var planeNormal = _relativeUp;
        
        var yaw = freeLookCamera.eulerAngles.y;

        var dot = Vector3.Dot(Vector3.Project(planeNormal, Vector3.up), Vector3.up);
        _facingUp = dot > 0 || Mathf.Approximately(dot, 0);

        if (_previousFacingUp != _facingUp)
        {
            var angle = Vector3.Angle(planeNormal, _previousFacingUp ? Vector3.up : Vector3.down);
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

        var planeRotation = Quaternion.FromToRotation(worldUp, planeNormal);
        var playerRotation = Quaternion.AngleAxis(yaw, worldUp);
        var movementForward = planeRotation * (playerRotation * Vector3.forward);
        var movementRotation = Quaternion.LookRotation(movementForward, planeNormal);
        var axisMovement = movementRotation * _inputMovement;

        Debug.DrawRay(transform.position, movementForward, Color.blue);

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
        var closest = new CustomRaycastHits
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

        _relativeUp = closest.Normal;
        transform.position = closest.Point + (_relativeUp.normalized * (0.25f + groundOffset));
    }
    
    private List<CustomRaycastHits> GetOverlappingHits()
    {
        var overlappingColliders = new Collider[5];
        var hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, 
            colliderCheckRadius, 
            overlappingColliders, 
            _environmentMask);

        var rayCasts = new List<CustomRaycastHits>();

        for (var i = 0; i < hitCount; i++)
        {
            var rayCast = new CustomRaycastHits
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
