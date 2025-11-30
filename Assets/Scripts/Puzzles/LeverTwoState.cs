using System.Collections.Generic;
using UnityEngine;

public class LeverTwoState : MonoBehaviour, ILevers
{
    public enum State
    {
        Max,
        Neutral ,
        Min
    }

    private State _previousState;
    [SerializeField]
    private State currentState = State.Neutral;
    private HingeJoint _joint;
    private JointMotor _motor;
    private JointSpring _spring;
    
    [SerializeField]
    private List<GameObject> debugStateObject;
    
    [SerializeField] private float stateThreshold = 10f;
    [SerializeField] private float motorSpeed = 10000f;
    [SerializeField] private float motorForce = 1f;
    [SerializeField] private bool stopGrippingUponActivation;
    
    private void Awake()
    {
        _joint = GetComponent<HingeJoint>();
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
            currentState = State.Max;
        }
        else if (angle <= min + stateThreshold)
        {
            currentState = State.Min;
        }
        else
        {
            currentState = State.Neutral;
        }

        if (_previousState != currentState)
        {
            if (stopGrippingUponActivation && currentState != State.Neutral)
            {
                PackagePickupController.Instance.StopGrippingLever();
            }

            switch (currentState)
            {
                case State.Min:
                    debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOff());
                    break;
                case State.Max:
                    debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOn());
                    break;
            }
        }

        _previousState = currentState;
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
            currentState switch
            {
                State.Max => _joint.limits.max,
                State.Min => _joint.limits.min,
                _ => 0f
            };
            
        _joint.spring = _spring;
    }
    
    // It should stop gripping if theres a trigger related to camera
}
