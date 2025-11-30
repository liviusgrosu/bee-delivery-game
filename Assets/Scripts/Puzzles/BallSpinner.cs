using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpinner : MonoBehaviour, IToggleObjects
{
    public enum State
    {
        Top,
        Bottom
    } 
    
    private State _currentState = State.Top;
    
    [SerializeField]
    private CameraFollowTargetTemp cameraFollowTarget;
    
    [SerializeField] private float animationTime = 7.5f;
    [SerializeField] private float rotationAmount = 90f;
    private Quaternion _startRotation, _endRotation;
    
    [HideInInspector]
    public Rigidbody BallRigidbody;
    
    private void Start()
    {
        _startRotation = transform.rotation;
        _endRotation = transform.rotation * Quaternion.Euler(0, -rotationAmount, 0);
    }

    private void Update()
    {
        if (!BallRigidbody)
        {
            return;
        }

        if (_currentState == State.Top && BallRigidbody.linearVelocity.magnitude < 1f)
        {
            cameraFollowTarget.EndFollowing();
        }
    }
    
    public void TriggerOn()
    {
        StopAllCoroutines();
        _currentState = State.Bottom;
        // Start showing the camera
        cameraFollowTarget.StartFollowing(BallRigidbody.gameObject);
        StartCoroutine(RotateTowards(_endRotation));
    }

    public void TriggerOff()
    {
        StopAllCoroutines();
        _currentState = State.Top;
        StartCoroutine(RotateTowards(_startRotation));
    }

    private IEnumerator RotateTowards(Quaternion target)
    {
        var timePassed = 0f;
        var startRotation = transform.rotation;
        while (timePassed < animationTime)
        {
            var step = timePassed / animationTime;
            transform.rotation = Quaternion.Lerp(startRotation, target, step);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }

    // Lose the camera when the ball is in the cup and rotated at the start
    // Start the camera again when its spinning again
}
