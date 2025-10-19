using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRaycastHit
{
    public Collider Collider { get; set; }
    public Vector3 Point { get; set; }
    public Vector3 Normal { get; set; }
    public float Distance { get; set; }
}

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    protected bool IsGrounded;
    private bool IsTakingOff;

    [Tooltip("Radius of collider sphere")]
    [SerializeField] 
    protected float colliderCheckRadius = 0.35f;
    
    protected int EnvironmentMask;

    private BeeFlappingAnimation flappingAnimation;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        flappingAnimation = GetComponentInChildren<BeeFlappingAnimation>();
    }

    private void Start()
    {
        EnvironmentMask = LayerMask.GetMask("Environment");
    }

    private void Update()
    {
        // Check to see if the player hits a wall or ground
        // If so, then we need to trigger the landing animation
        if (IsTakingOff)
        {
            return;
        }
        
        if (IsGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine(TakeOff());
            }
            return;
        }
        
        var (hitSomething, hit) = GetGroundedState();
        if (hitSomething)
        {
            IsGrounded = true;
            flappingAnimation.enabled = false;

            PlayerFlyingMovement.Instance.ResetModelRotation();
            
            PlayerFlyingMovement.Instance.enabled = false;
            PlayerWalkingMovement.Instance.enabled = true;
        }
    }
    
    protected (bool, CustomRaycastHit) GetGroundedState()
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
        
        return (hitSomething, closest);
    }
    
    private List<CustomRaycastHit> GetOverlappingHits()
    {
        var overlappingColliders = new Collider[5];
        var hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, 
            colliderCheckRadius, 
            overlappingColliders, 
            EnvironmentMask);

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

    private IEnumerator TakeOff()
    {
        const float duration = 0.1f;
        var elapsed = 0f;

        var currentPos = transform.position;
        var finalPos = transform.position + PlayerWalkingMovement.Instance.RelativeUp;
        
        PlayerWalkingMovement.Instance.enabled = false;
        IsTakingOff = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Slerp(currentPos, finalPos, elapsed / duration);
            yield return null;
        }

        PlayerFlyingMovement.Instance.enabled = true;
        flappingAnimation.enabled = true;
        IsGrounded = false;
        IsTakingOff = false;
    }
}
