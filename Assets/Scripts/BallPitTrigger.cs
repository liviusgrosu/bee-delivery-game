using UnityEngine;

public class BallPitTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject debugStateObject;

    [SerializeField] private Transform triggeringBall;
    private bool _isOn;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != triggeringBall)
        {
            return;
        }
        debugStateObject.GetComponent<ITriggerObjects>().TriggerOn();
        _isOn = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform != triggeringBall)
        {
            return;
        }
        debugStateObject.GetComponent<ITriggerObjects>().TriggerOff();
        _isOn = false;
    }
}
