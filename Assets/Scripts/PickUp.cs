using TMPro;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Transform _potentialPickUpItem;
    private Transform _pickedUpItem;
    
    public TextMeshProUGUI PickUpText;

    private void Start()
    {
        PickUpText.enabled = false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_potentialPickUpItem && !_pickedUpItem)
            {
                _pickedUpItem =  _potentialPickUpItem;
                var rb = _pickedUpItem.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
                _pickedUpItem.parent = transform;
                _pickedUpItem.position = transform.position;
                
                PickUpText.text = "Press 'F' to drop";
            }
            
            else if (_pickedUpItem)
            {
                var rb = _pickedUpItem.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                }
                _pickedUpItem.parent = null;
                _pickedUpItem = null;
                
                PickUpText.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pickedUpItem != null) return;

        if (other.CompareTag("Box"))
        {
            PickUpText.enabled = true;
            PickUpText.text = "Press 'F' to pickup";
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (_pickedUpItem != null) return;

        if (other.CompareTag("Box"))
        {
            _potentialPickUpItem = other.transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_pickedUpItem != null) return;

        if (other.CompareTag("Box"))
        {
            PickUpText.enabled = false;
        }
    }
}
