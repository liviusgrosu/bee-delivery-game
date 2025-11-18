using UnityEngine;

public class BallPitTrigger : MonoBehaviour
{
    [SerializeField]
    private DebugStateObject debugStateObject;

    [SerializeField] private Transform triggeringBall;
    private bool _isOn;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != triggeringBall)
        {
            return;
        }
        debugStateObject.TriggerOn();
        _isOn = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform != triggeringBall)
        {
            return;
        }
        debugStateObject.TriggerOff();
        _isOn = false;
    }
}
