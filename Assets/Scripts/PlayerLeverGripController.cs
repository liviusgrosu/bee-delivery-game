using UnityEngine;

public class PlayerLeverGripController : MonoBehaviour
{
    public static PlayerLeverGripController Instance;

    private ILevers _leverTwoState;
    private Transform _anchor;
    private JointMotor _motor;

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
    
    public void Init(ILevers leverTwoState, Transform anchor)
    {
        _leverTwoState = leverTwoState;
        _anchor = anchor;
        leverTwoState.StartGrip();
    }

    public void Disable()
    {
        _anchor = null;
        _leverTwoState.EndGrip();
        _leverTwoState = null;
    }
    
    private void Update()
    {
        if (!_anchor)
        {
            return;
        }
        transform.position = _anchor.position;
        _leverTwoState.UpdateSpeed();
    }
}
