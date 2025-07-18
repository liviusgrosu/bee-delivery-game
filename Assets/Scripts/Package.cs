using UnityEngine;

public class Package : MonoBehaviour
{
    [HideInInspector]
    private float _minWeight = 1f;
    [HideInInspector]
    private float _maxWeight = 5f;

    public float Weight = 1f;
    
    private Rigidbody _rb;
    private Collider _col;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        Weight = Random.Range(_minWeight, _maxWeight);
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
