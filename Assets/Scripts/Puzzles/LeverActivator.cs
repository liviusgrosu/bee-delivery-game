using UnityEngine;

public class LeverActivator : MonoBehaviour, ILevers
{
    private HingeJoint _joint;
    private JointMotor _motor;
    private JointSpring _spring;
    [SerializeField] private GameObject trigger;
    
    [SerializeField] private float stateThreshold = 10f;
    [SerializeField] private float motorSpeed = 10000f;
    [SerializeField] private float motorForce = 1f;
    
    private void Awake()
    {
        _joint = GetComponent<HingeJoint>();
        ResetSpringState();
    }
    
    public void UpdateSpeed()
    {
        var input = motorSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        _motor.targetVelocity = input;
        _joint.motor = _motor;
        
        var angle = _joint.angle;
        var max = _joint.limits.max * 2f;
        
        //Debug.Log($"angle: {angle}, max: {max}, threshold: {max - stateThreshold}");
        
        if (angle >= max - stateThreshold) 
        {
            PackagePickupController.Instance.StopGrippingLever();
            trigger.GetComponent<ITriggerObjects>().Trigger();
        }
    }
    
    public void StartGrip()
    {
        _joint.useSpring = false;
        _joint.useMotor = true;
        _motor.force = motorForce;
        _joint.motor = _motor;
    }
    
    public void EndGrip()
    {
        _motor.force = 0;
        _joint.motor = _motor;
        _joint.useMotor = false;
        ResetSpringState();
    }
    
    private void ResetSpringState()
    {
        _joint.useSpring = true;
        _spring.spring = 50f;
        _spring.targetPosition = _joint.limits.min;
        _joint.spring = _spring;
    }
}
