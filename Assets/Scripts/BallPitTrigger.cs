using System.Collections.Generic;
using UnityEngine;

public class BallPitTrigger : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> debugStateObject;
    // Once triggered, cant be turned off
    [SerializeField] private bool onceOnly;
    
    private bool _isOn;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Puzzle Ball"))
        {
            return;
        }
        debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOn());
        _isOn = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Puzzle Ball"))
        {
            return;
        }

        if (!onceOnly)
        {
            debugStateObject.ForEach(go => go.GetComponent<IToggleObjects>().TriggerOff());
            _isOn = false;
        }
    }
}
