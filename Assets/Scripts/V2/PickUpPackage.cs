
using UnityEngine;

public class PickUpPackage : MonoBehaviour
{
    public static PickUpPackage Instance;
    
    private Transform _potentialPickUpItem;
    private Transform _pickedUpItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_potentialPickUpItem && !_pickedUpItem)
            {
                BeeAnimation.Instance.PickUp();
                
                _pickedUpItem =  _potentialPickUpItem;
                var packageComponent = _pickedUpItem.GetComponent<Package>();
                
                if (packageComponent)
                {
                    packageComponent.PickUp();
                }
                
                _pickedUpItem.parent = transform;
                //_pickedUpItem.position = transform.position;
            }
            
            else if (_pickedUpItem)
            {
                BeeAnimation.Instance.DropOff();
                var package = _pickedUpItem.GetComponent<Package>();
                
                if (package)
                {
                    package.DropOff();
                }
                _pickedUpItem.parent = null;
                _pickedUpItem = null;
                _potentialPickUpItem = null;
            }
        }
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
