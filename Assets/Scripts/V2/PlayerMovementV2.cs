using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
    public static PlayerMovementV2 Instance { get; private set; }

    public float moveSpeed;

    public Camera freeLookCamera;
    private float currentHeight;
    private float xRotation;
    
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
        currentHeight = transform.position.y;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        xRotation = freeLookCamera.transform.rotation.eulerAngles.x;
        
        if (Input.GetKey(KeyCode.W))
        {
            MoveCharacter();
        }
        else
        {
            DisableMovement();
        }
        
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
    }

    private void MoveCharacter()
    {
        var cameraForward = new Vector3(
            freeLookCamera.transform.forward.x,
            0, 
            freeLookCamera.transform.forward.z
        );
        
        transform.rotation = Quaternion.LookRotation(cameraForward);
        transform.Rotate(new Vector3(xRotation, 0, 0), Space.Self);
        
        var forward = freeLookCamera.transform.forward;
        var flyDirection = forward.normalized;
        
        currentHeight += flyDirection.y * moveSpeed * Time.deltaTime;
        
        transform.position += flyDirection * (moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
    }

    private void DisableMovement()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
