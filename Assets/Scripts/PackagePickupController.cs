
using System;
using System.Collections;
using UnityEngine;

public class PackagePickupController : MonoBehaviour
{
    public static PackagePickupController Instance;
    [SerializeField] private Transform BeeModelForward;
    [SerializeField] private Transform packageClampPosition;
    // Packages
    private Transform _potentialPickUpItem;
    private Transform _pickedUpItem;
    // Grips
    private Transform _potentialGrip;
    private Transform _grippedItem;
    [SerializeField] private float largePackageThreshold = 5f;
    [SerializeField] private float maxPackageWeight = 10f;
    [SerializeField] private float packageDamageThreshold = 10f;
    [SerializeField] private float largePackageRotationSpeed = 20f;
    // Packages
    public bool IsHoldingPackage => _pickedUpItem != null;
    public Package CurrentPackageComp;
    
    public static event Action PickUpPackageEvent;
    public static event Action DropPackageEvent;
    
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
            // Grippable objects
            if (_potentialGrip && !_grippedItem)
            {
                StartGripping();
            }
            else if (_grippedItem)
            {
                StopGripping();
            }
            
            // Packages
            if (_potentialPickUpItem && !_pickedUpItem)
            {
                PickUpPackage();
            }
            else if (_pickedUpItem)
            {
                DropPackage();
            }
        }

        if (_pickedUpItem && IsHoldingPackage)
        {
            var rotateInput = (Input.GetKey(KeyCode.Q) ? 1f : 0f) 
                           + (Input.GetKey(KeyCode.E) ? -1f : 0f); 
            _pickedUpItem.Rotate(_pickedUpItem.up, rotateInput * largePackageRotationSpeed * Time.deltaTime);
        }
    }

    private void PickUpPackage()
    {
        _pickedUpItem =  _potentialPickUpItem;
        CurrentPackageComp = _pickedUpItem.GetComponent<Package>();
                
        if (CurrentPackageComp )
        {
            if (!CurrentPackageComp .Interactable)
            {
                return;
            }
            CurrentPackageComp .PickUp();
        }
        
        PickUpPackageEvent?.Invoke();
        GameManager.Instance.PickedUpPackage();
        BeeAnimation.Instance.PickUp();
        // Doing this so the package doesn't move the player during this process
        // Provided that the model has a Vector3.eular rotation of Vector3.zero
        StartCoroutine(PickUpPackageAnimation(_pickedUpItem.position, CurrentPackageComp .PickupDistance));
    }

    private IEnumerator PickUpPackageAnimation(Vector3 start, Vector3 offset)
    {
        CurrentPackageComp.Collider.enabled = false;
        var timePassed = 0f;
        var animationTime = 0.2f;
        while (timePassed < animationTime)
        {
            var step = timePassed / animationTime;
            _pickedUpItem.position = Vector3.Lerp(start, packageClampPosition.position + offset, step);
            timePassed += Time.deltaTime;
            yield return null;
        }
        _pickedUpItem.parent = packageClampPosition;
        CurrentPackageComp.Collider.enabled = true;
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
        DropPackageEvent?.Invoke();
        BeeAnimation.Instance.DropOff();
        CurrentPackageComp.DropOff();
        
        CurrentPackageComp = null;
        _pickedUpItem.parent = null;
        _pickedUpItem = null;
        _potentialPickUpItem = null;
    }
    
    public bool IsHoldingLargePackage()
    {
        return _pickedUpItem && CurrentPackageComp.CurrentWeight >= largePackageThreshold;
    }

    public float GetCarryingWeightPerc()
    {
        return !_pickedUpItem 
            ? 1f 
            : Mathf.Clamp01((maxPackageWeight - CurrentPackageComp.CurrentWeight) / maxPackageWeight);
    }

    private void StartGripping()
    {
        _grippedItem = _potentialGrip;
        // Turn off all movement scripts
        PlayerFlyingMovement.Instance.enabled = false;
        var lever = _grippedItem.GetComponent<Lever>();
        var gripPosition = _grippedItem.Find("Grip Position");
        // Turn on the grip controller
        PlayerGripController.Instance.enabled = true;
        PlayerGripController.Instance.Init(lever, gripPosition);
        // Make model face a certain direction
        // Make sure to store that direction when we stop gripping
    }

    private void StopGripping()
    {
        PlayerFlyingMovement.Instance.enabled = true;
        _grippedItem = _potentialGrip = null;
        PlayerGripController.Instance.Disable();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            _potentialPickUpItem = other.transform;
        }
        else if (other.CompareTag("Grip"))
        {
            _potentialGrip = other.transform;            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            _potentialPickUpItem = null;
        }
        else if (other.CompareTag("Grip"))
        {
            _potentialGrip = null;
        }
    }
}
