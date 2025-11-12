using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera defaultCameraView;
    [SerializeField] private CinemachineCamera largePackageCameraView;

    private void Start()
    {
        PackagePickupController.PickUpPackageEvent += SwitchToLargePackageView;
        PackagePickupController.DropPackageEvent += SwitchToDefaultView;
        SwitchToDefaultView();
    }
    
    private void SwitchToDefaultView()
    {
        defaultCameraView.Priority.Value = 1;
        largePackageCameraView.Priority.Value = 0;
    }
    
    private void SwitchToLargePackageView()
    {
        defaultCameraView.Priority.Value = 0;
        largePackageCameraView.Priority.Value = 1;
        /*defaultCameraView.SetActive(false);
        largePackageCameraView.SetActive(true);*/
    }
}
