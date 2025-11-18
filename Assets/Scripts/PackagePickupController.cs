
using System;
using System.Collections;
using UnityEngine;

public class PackagePickupController : MonoBehaviour
{
    public static PackagePickupController Instance;
    [SerializeField] private Transform BeeModel;
    [SerializeField] private Transform BeeModelForward;
    [SerializeField] private Transform packageClampPosition;
    
    // Packages
    private Transform _potentialPickUpItem;
    private Transform _pickedUpItem;
    // Puzzle Ball
    private Transform _potentialPuzzleBallItem;
    private Transform _grippedPuzzleBall;
    // Lever
    private Transform _potentialLever;
    private Transform _grippedLever;
    // Sliding Box
    private Transform _potentialSlidingBox;
    private Transform _grippedSlidingBox;
    
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
            // Lever
            if (_potentialLever && !_grippedLever)
            {
                StartGrippingLever();
            }
            else if (_grippedLever)
            {
                StopGrippingLever();
            }

            // Puzzle Box
            if (_potentialPuzzleBallItem && !_grippedPuzzleBall)
            {
                StartGrippingPuzzleBall();
            }
            else if (_grippedPuzzleBall)
            {
                StopGrippingPuzzleBall();
            }
            
            // Sliding Box
            if (_potentialSlidingBox && !_grippedSlidingBox)
            {
                StartGrippingSlidingBox();
            }
            else if (_grippedSlidingBox)
            {
                StopGrippingSlidingBox();
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
                
        if (CurrentPackageComp)
        {
            if (!CurrentPackageComp.Interactable)
            {
                return;
            }
            CurrentPackageComp.PickUp();
        }
        
        PickUpPackageEvent?.Invoke();
        GameManager.Instance.PickedUpPackage();
        BeeAnimation.Instance.PickUp();
        // Doing this so the package doesn't move the player during this process
        // Provided that the model has a Vector3.eular rotation of Vector3.zero
        StartCoroutine(PickUpPackageAnimation(_pickedUpItem.position, CurrentPackageComp.PickupDistance));
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
        _pickedUpItem = _potentialPickUpItem = null;
    }
    
    public bool IsHoldingLargePackage()
    {
        return _pickedUpItem && CurrentPackageComp.CurrentWeight >= largePackageThreshold;
    }

    public bool IsHoldingLargeObject()
    {
        return _grippedPuzzleBall;
    }

    public float GetCarryingWeightPerc()
    {
        return !_pickedUpItem 
            ? 1f 
            : Mathf.Clamp01((maxPackageWeight - CurrentPackageComp.CurrentWeight) / maxPackageWeight);
    }

    private void StartGrippingLever()
    {
        _grippedLever = _potentialLever;
        // Turn off all movement scripts
        PlayerFlyingMovement.Instance.enabled = false;
        var lever = _grippedLever.GetComponent<Lever>();
        var gripPosition = _grippedLever.Find("Grip Position");
        // Turn on the grip controller
        PlayerLeverGripController.Instance.enabled = true;
        PlayerLeverGripController.Instance.Init(lever, gripPosition);
        // Make model face a certain direction
        // Make sure to store that direction when we stop gripping
    }

    private void StopGrippingLever()
    {
        PlayerFlyingMovement.Instance.enabled = true;
        _grippedLever = _potentialLever = null;
        PlayerLeverGripController.Instance.Disable();
    }

    private void StartGrippingSlidingBox()
    {
        _grippedSlidingBox = _potentialSlidingBox;
        PlayerFlyingMovement.Instance.enabled = false;
        var slidingBox = _grippedSlidingBox.GetComponent<SlidingBox>();
        var gripPosition = _grippedSlidingBox.Find("Grip Position");
        PlayerSlidingBoxGripController.Instance.enabled = true;
        PlayerSlidingBoxGripController.Instance.Init(slidingBox, gripPosition);
    }

    private void StopGrippingSlidingBox()
    {
        PlayerFlyingMovement.Instance.enabled = true;
        _grippedSlidingBox = _potentialSlidingBox = null;
        PlayerSlidingBoxGripController.Instance.Disable();
    }

    private void StartGrippingPuzzleBall()
    {
        _grippedPuzzleBall = _potentialPuzzleBallItem;
        var puzzleBall = _grippedPuzzleBall.GetComponent<PuzzleBall>();
        puzzleBall.Pickup(BeeModel.position, packageClampPosition);
        PickUpPackageEvent?.Invoke();
        BeeAnimation.Instance.PickUp();
    }

    private void StopGrippingPuzzleBall()
    {
        DropPackageEvent?.Invoke();
        BeeAnimation.Instance.DropOff();
        var puzzleBall = _grippedPuzzleBall.GetComponent<PuzzleBall>();
        puzzleBall.DropOff();
        _grippedPuzzleBall = _potentialPuzzleBallItem = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entering trigger with tag {other.tag}");
        if (other.CompareTag("Package"))
        {
            _potentialPickUpItem = other.transform;
        }
        else if (other.CompareTag("Puzzle Ball"))
        {
            _potentialPuzzleBallItem = other.transform;
        }
        else if (other.CompareTag("Lever"))
        {
            _potentialLever = other.transform;            
        }
        else if (other.CompareTag("Sliding Box"))
        {
            _potentialSlidingBox = other.transform;            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            _potentialPickUpItem = null;
        }
        else if (other.CompareTag("Puzzle Ball"))
        {
            _potentialPuzzleBallItem = null;
        }
        else if (other.CompareTag("Lever"))
        {
            _potentialLever = null;
        }
        else if (other.CompareTag("Sliding Box"))
        {
            _potentialSlidingBox = null;
        }
    }
}
