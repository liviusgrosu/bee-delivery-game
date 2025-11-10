
using System.Collections;
using UnityEngine;

public class PackagePickupController : MonoBehaviour
{
    public static PackagePickupController Instance;
    [SerializeField] private Transform packageClampPosition;
    private Transform _potentialPickUpItem;
    private Transform _pickedUpItem;
    [SerializeField] private float largePackageThreshold = 5f;
    [SerializeField] private float maxPackageWeight = 10f;
    public bool IsHoldingPackage => _pickedUpItem != null;
    private Package _currentPackageComp;

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
        _currentPackageComp = _pickedUpItem.GetComponent<Package>();
                
        if (_currentPackageComp )
        {
            if (!_currentPackageComp .Interactable)
            {
                return;
            }
            _currentPackageComp .PickUp();
        }
        
        GameManager.Instance.PickedUpPackage();
        BeeAnimation.Instance.PickUp();
        
        _pickedUpItem.parent = packageClampPosition;
        // Provided that the model has a Vector3.eular rotation of Vector3.zero
        _pickedUpItem.rotation = packageClampPosition.rotation;
        StartCoroutine(PickUpPackageAnimation(_pickedUpItem.position, _currentPackageComp .PickupDistance));
    }

    private IEnumerator PickUpPackageAnimation(Vector3 start, Vector3 offset)
    {
        var timePassed = 0f;
        var animationTime = 0.1f;
        while (timePassed < animationTime)
        {
            var step = timePassed / animationTime;
            _pickedUpItem.position = Vector3.Lerp(start, packageClampPosition.position + offset, step);
            timePassed += Time.deltaTime;
            yield return null;
        }
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
        _currentPackageComp .DropOff();
        
        _currentPackageComp = null;
        _pickedUpItem.parent = null;
        _pickedUpItem = null;
        _potentialPickUpItem = null;
    }
    
    
    public bool IsHoldingLargePackage()
    {
        return _pickedUpItem && _currentPackageComp.CurrentWeight >= largePackageThreshold;
    }

    public float GetCarryingWeightPerc()
    {
        return !_pickedUpItem 
            ? 1f 
            : Mathf.Clamp01((maxPackageWeight - _currentPackageComp.CurrentWeight) / maxPackageWeight);
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
