using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField]
    private float _minWeight = 1f;
    [SerializeField]
    private float _maxWeight = 1f;

    public float Weight = 1f;
    public float PayOut = 1f;
    
    private Rigidbody _rb;
    private Collider _col;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    public void PickUp()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _col.enabled = false;
    }

    public void DropOff()
    {
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _col.enabled = true;
    }
}
