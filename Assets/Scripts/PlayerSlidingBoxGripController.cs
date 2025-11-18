using UnityEngine;

public class PlayerSlidingBoxGripController : MonoBehaviour
{
    public static PlayerSlidingBoxGripController Instance;
    private Collider _collider;
    
    private SlidingBox _slidingBox;
    private Transform _anchor;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _collider = GetComponent<Collider>();
        // Auto-start disabled
        enabled = false;
    }
    
    public void Init(SlidingBox slidingBox, Transform anchor)
    {
        // TODO: lock the rotation of the player
        _slidingBox = slidingBox;
        _anchor = anchor;
        _slidingBox.StartGrip();
    }
    
    public void Disable()
    {
        _anchor = null;
        _slidingBox.EndGrip();
        _slidingBox = null;
    }

    private void Update()
    {
        if (!_anchor)
        {
            return;
        }

        transform.position = _anchor.position;
    }

    private void FixedUpdate()
    {
        if (!_anchor)
        {
            return;
        }
        
        _slidingBox.UpdateSpeed();
    }
}
