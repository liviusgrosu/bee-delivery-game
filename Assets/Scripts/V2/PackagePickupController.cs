
using UnityEngine;

public class PackagePickupController : MonoBehaviour
{
    public static PackagePickupController Instance;
    [SerializeField] private Transform packageClampPosition;
    private Transform _potentialPickUpItem;
    private Transform _pickedUpItem;
    public bool IsHoldingPackage => _pickedUpItem != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        PlayerFlyingMovement.StunnedEvent += DropPackage;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_potentialPickUpItem && !_pickedUpItem)
            {
                PickUpPackage();
            }
            else if (_pickedUpItem)
            {
                DropPackage();
            }
        }
    }

    private void PickUpPackage()
    {
        _pickedUpItem =  _potentialPickUpItem;
        var packageComponent = _pickedUpItem.GetComponent<PackageV2>();
                
        if (packageComponent)
        {
            if (!packageComponent.Interactable)
            {
                return;
            }
            packageComponent.PickUp();
        }
        
        GameManagerV1.Instance.PickedUpPackage();
        BeeAnimation.Instance.PickUp();
                        
        _pickedUpItem.parent = packageClampPosition;
        _pickedUpItem.position = packageClampPosition.position;
        // Provided that the model has a Vector3.eular rotation of Vector3.zero
        _pickedUpItem.rotation = packageClampPosition.rotation;
    }

    private void DropPackage(bool state)
    {
        if (!state || !_pickedUpItem)
        {
            return;
        }
        DropPackage();
    } 
    
    private void DropPackage()
    {
        BeeAnimation.Instance.DropOff();
        var package = _pickedUpItem.GetComponent<PackageV2>();
                
        if (package)
        {
            package.DropOff();
        }
        _pickedUpItem.parent = null;
        _pickedUpItem = null;
        _potentialPickUpItem = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            _potentialPickUpItem = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            _potentialPickUpItem = null;
        }
    }
}
