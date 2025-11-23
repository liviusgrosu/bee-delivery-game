using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera defaultCameraView;
    [SerializeField] private CinemachineCamera largePackageCameraView;
    [SerializeField] private CinemachineCamera puzzleBoxCameraView;
    
    private void Start()
    {
        PackagePickupController.PickUpPackageEvent += SwitchToLargePackageView;
        PackagePickupController.DropPackageEvent += SwitchToDefaultView;
        PackagePickupController.GripSlidingBoxEvent += SwitchToSlidingBoxView;
        
        SwitchToDefaultView();
    }
    
    private void SwitchToDefaultView()
    {
        defaultCameraView.Priority.Value = 1;
        largePackageCameraView.Priority.Value = 0;
        puzzleBoxCameraView.Priority.Value = 0;
    }
    
    private void SwitchToLargePackageView()
    {
        defaultCameraView.Priority.Value = 0;
        largePackageCameraView.Priority.Value = 1;
        puzzleBoxCameraView.Priority.Value = 0;
    }

    private void SwitchToSlidingBoxView()
    {
        defaultCameraView.Priority.Value = 0;
        largePackageCameraView.Priority.Value = 0;
        puzzleBoxCameraView.Priority.Value = 1;
    }
}
