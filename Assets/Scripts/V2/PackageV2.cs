using UnityEngine;

public class PackageV2 : MonoBehaviour
{
    public float Health;
    public MeshRenderer _boxRenderer;
    private Rigidbody _rigidBody;
    private Collider _collider;
    private float _currentSpeed;
    private bool _takenDamage;
    public PackageConditions[] PackageConditions;
    public bool Interactable = true;
    public float PayOut = 1f;
    
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    
    private void Update()
    {
        _currentSpeed = _rigidBody.linearVelocity.magnitude;
        if (Mathf.Approximately(_currentSpeed, 0f))
        {
            _takenDamage = false;
        }
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (_takenDamage || other.gameObject.layer != LayerMask.NameToLayer("Environment"))
        {
            return;
        }
        _takenDamage = true;
        Health -= _currentSpeed;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        UpdateCondition();
    }

    private void UpdateCondition()
    {
        foreach(var condition in PackageConditions)
        {
            if (Health <= condition.Health)
            {
                _boxRenderer.material = condition.Material;
                continue;
            }
            return;
        }
    }
    
    public void PickUp()
    {
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
        _collider.enabled = false;
    }

    public void DropOff()
    {
        _rigidBody.useGravity = true;
        _rigidBody.isKinematic = false;
        _collider.enabled = true;
    }
}
