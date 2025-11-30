using System.Collections.Generic;
using UnityEngine;

public class BallPitTrigger : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> debugStateObject;
    [SerializeField]
    private List<GameObject> debugTriggerObjects;
    // Once triggered, cant be turned off
    [SerializeField] private bool onceOnly;
    [SerializeField] private bool shouldStabilizedBall;
    
    private bool _isOn;
    
    private Rigidbody _ballRigidbody;

    private void Update()
    {
        if (!_ballRigidbody)
        {
            return;
        }

        if (!_isOn && _ballRigidbody.linearVelocity.magnitude < 1f)
        {
            debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOn());
            debugTriggerObjects.ForEach(go => go.GetComponent<ITriggerObjects>().Trigger());
            _isOn = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Puzzle Ball"))
        {
            return;
        }

        if (shouldStabilizedBall)
        {
            _ballRigidbody = other.GetComponent<Rigidbody>();
            return;
        }
        
        debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOn());
        debugTriggerObjects.ForEach(go => go.GetComponent<ITriggerObjects>().Trigger());
        _isOn = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Puzzle Ball"))
        {
            return;
        }
        
        if (shouldStabilizedBall)
        {
            _ballRigidbody = null;
            return;
        }

        // Don't trigger things off since its been already triggered
        if (!onceOnly)
        {
            debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOff());
            _isOn = false;
        }
    }
}
