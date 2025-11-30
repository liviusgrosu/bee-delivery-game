using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BallSpawner : MonoBehaviour, ITriggerObjects
{
    [SerializeField]
    private GameObject ballPrefab;
    
    [SerializeField]
    private CameraFollowTargetTemp cameraFollowTarget;

    public void Trigger()
    {
        var ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        if (cameraFollowTarget)
        {
            cameraFollowTarget.StartFollowing(ball);
        }
    }
}
