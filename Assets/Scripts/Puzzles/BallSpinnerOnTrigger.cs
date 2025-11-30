using System;
using UnityEngine;

public class BallSpinnerOnTrigger : MonoBehaviour
{
    private BallSpinner _ballSpinner;

    private void Start()
    {
        _ballSpinner = transform.parent.GetComponent<BallSpinner>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Puzzle Ball"))
        {
            return;
        }
        
        _ballSpinner.BallRigidbody = other.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Puzzle Ball"))
        {
            return;
        }

        _ballSpinner.BallRigidbody = null;
    }
}
