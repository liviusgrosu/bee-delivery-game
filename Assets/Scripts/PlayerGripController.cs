using UnityEngine;

public class PlayerGripController : MonoBehaviour
{
    public static PlayerGripController Instance;

    private Lever _lever;
    private Transform _anchor;
    private JointMotor _motor;
    [SerializeField] private float motorSpeed = 2f;
    [SerializeField] private float motorForce = 100f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Auto-start disabled
        enabled = false;
    }
    
    public void Init(Lever lever, Transform anchor)
    {
        _lever = lever;
        _anchor = anchor;
        lever.StartGrip();
    }

    public void Disable()
    {
        _anchor = null;
        _lever.EndGrip();
        _lever = null;
    }
    
    private void Update()
    {
        if (!_anchor)
        {
            return;
        }
        transform.position = _anchor.position;
        var input = motorSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        _lever.UpdateSpeed(input);
    }
}
