using System;
using Unity.Cinemachine;
using UnityEngine;


public class CameraFollowTargetTemp : MonoBehaviour, ITriggerObjects
{
    private bool _isFocused;
    private CinemachineCamera _camera;
    private GameObject _anchor;
    
    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }

    private void Update()
    {
        if (!_isFocused || _anchor)
        {
            return;
        }

        EndFollowing();
    }

    public void Trigger()
    {
        EndFollowing();
    }

    public void StartFollowing(GameObject target)
    {
        PlayerFlyingMovement.Instance.ToggleCameraFocus(true);
        _anchor = target;
        _camera.Follow = _anchor.transform;
        CameraManager.Instance.SwitchToSpecificCamera(_camera);
        _isFocused = true;
    }
    
    public void EndFollowing()
    {
        PlayerFlyingMovement.Instance.ToggleCameraFocus(true);
        CameraManager.Instance.SwitchToDefaultView();
        _isFocused = false;
        _anchor = null;
    }
}
