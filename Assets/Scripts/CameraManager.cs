using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [SerializeField] private CinemachineCamera defaultCameraView;
    [SerializeField] private CinemachineCamera largePackageCameraView;
    [SerializeField] private CinemachineCamera puzzleBoxCameraView;

    private CinemachineCamera _specificCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        PackagePickupController.PickUpPackageEvent += SwitchToLargePackageView;
        PackagePickupController.DropPackageEvent += SwitchToDefaultView;
        PackagePickupController.GripSlidingBoxEvent += SwitchToSlidingBoxView;
        
        SwitchToDefaultView();
    }
    
    public void SwitchToDefaultView()
    {
        CheckAndRemoveSpecificCamera();
        defaultCameraView.Priority.Value = 1;
        largePackageCameraView.Priority.Value = 0;
        puzzleBoxCameraView.Priority.Value = 0;
    }
    
    private void SwitchToLargePackageView()
    {
        CheckAndRemoveSpecificCamera();
        defaultCameraView.Priority.Value = 0;
        largePackageCameraView.Priority.Value = 1;
        puzzleBoxCameraView.Priority.Value = 0;
    }

    private void SwitchToSlidingBoxView()
    {
        CheckAndRemoveSpecificCamera();
        defaultCameraView.Priority.Value = 0;
        largePackageCameraView.Priority.Value = 0;
        puzzleBoxCameraView.Priority.Value = 1;
    }

    private void CheckAndRemoveSpecificCamera()
    {
        if (!_specificCamera)
        {
            return;
        }
        _specificCamera.Priority.Value = 0;
        _specificCamera = null;
    }
    
    public void SwitchToSpecificCamera(CinemachineCamera newCamera)
    {
        _specificCamera = newCamera;
        _specificCamera.Priority.Value = 1;
        defaultCameraView.Priority.Value = 0;
        largePackageCameraView.Priority.Value = 0;
        puzzleBoxCameraView.Priority.Value = 0;
    }
}
