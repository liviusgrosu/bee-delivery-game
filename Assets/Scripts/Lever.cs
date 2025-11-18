using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Lever : MonoBehaviour
{
    public enum State
    {
        Max,
        Neutral ,
        Min
    }

    private State _previousState;
    private State _currentState;
    private HingeJoint _joint;
    private JointMotor _motor;
    private JointSpring _spring;
    
    [SerializeField] private float stateThreshold = 10f;
    
    [SerializeField] private float motorSpeed = 10000f;
    [SerializeField] private float motorForce = 1f;

    private void Awake()
    {
        _joint = GetComponent<HingeJoint>();
        _currentState = State.Neutral;
        SetSpringState();
    }

    public void UpdateSpeed()
    {
        var input = motorSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        _motor.targetVelocity = input;
        _joint.motor = _motor;

        var angle = _joint.angle;
        var min = _joint.limits.min;
        var max = _joint.limits.max;

        if (angle >= max - stateThreshold) 
        {
            _currentState = State.Max;
        }
        else if (angle <= min + stateThreshold)
        {
            _currentState = State.Min;
        }
        else
        {
            _currentState = State.Neutral;
        }

        if (_previousState != _currentState)
        {
            // TODO: Trigger something...
        }

        _previousState = _currentState;
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
        SetSpringState();
    }

    private void SetSpringState()
    {
        _joint.useSpring = true;
        _spring.spring = 10000f;
        _spring.targetPosition = 
            _currentState switch
            {
                State.Max => _joint.limits.max,
                State.Min => _joint.limits.min,
                _ => 0f
            };
            
        _joint.spring = _spring;
    }
}
