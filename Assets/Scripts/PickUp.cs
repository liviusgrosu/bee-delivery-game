using TMPro;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Transform _potentialPickUpItem;
    public Transform PickedUpItem;
    
    public float CurrentWeight = 1f;
    

    private void Start()
    {
        UIManager.Instance.PickUpText.enabled = false; 
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_potentialPickUpItem && !PickedUpItem)
            {
                PickedUpItem =  _potentialPickUpItem;
                var package = PickedUpItem.GetComponent<Package>();
                
                if (package)
                {
                    package.PickUp();
                    CurrentWeight = package.Weight;
                }
                
                PickedUpItem.parent = transform;
                PickedUpItem.position = transform.position;
                
                UIManager.Instance.PickUpText.text = "Press 'F' to drop";
            }
            
            else if (PickedUpItem)
            {
                DropPackage();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PickedUpItem != null) return;

        if (other.CompareTag("Box"))
        {
            UIManager.Instance.PickUpText.enabled = true;
            UIManager.Instance.PickUpText.text = "Press 'F' to pickup";
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (PickedUpItem != null) return;

        if (other.CompareTag("Box"))
        {
            _potentialPickUpItem = other.transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (PickedUpItem != null) return;

        if (other.CompareTag("Box"))
        {
            UIManager.Instance.PickUpText.enabled = false; 
        }
    }

    public void DropPackage()
    {
        var package = PickedUpItem.GetComponent<Package>();
                
        if (package)
        {
            package.DropOff();
            CurrentWeight = 1;
        }
        PickedUpItem.parent = null;
        PickedUpItem = null;
                
        UIManager.Instance.PickUpText.enabled = false; 
    }
}
