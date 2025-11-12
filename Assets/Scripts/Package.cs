using System.Collections;
using UnityEngine;

public class Package : MonoBehaviour
{
    public float _currentHealth;
    public float MaxHP;
    public MeshRenderer _boxRenderer;
    private Rigidbody _rigidBody;
    public Collider Collider;
    private float _currentSpeed;
    private bool _takenDamage;
    public Transform PickUpPoint;
    public Vector3 PickupDistance;
    public PackageConditions[] PackageConditions;
    public bool Interactable = true;    
    public float PayOut = 1f;
    public float PotentialTip;
    public float CurrentWeight;

    private float _iFrameCurrentTime;
    public GameObject DustParticle;
    
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        PickupDistance = transform.position - PickUpPoint.position;
    }

    public void SetHealth(float hp)
    {
        MaxHP = _currentHealth = hp;
    }
    
    private void Update()
    {
        if (!_rigidBody)
        {
            return;
        }
        
        // This only applies to when the package is not picked up
        _currentSpeed = _rigidBody.linearVelocity.magnitude;
        if (Mathf.Approximately(_currentSpeed, 0f))
        {
            _takenDamage = false;
        }
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            return;
        }
        
        TakeDamage(_currentSpeed, other.contacts);
    }

    public void TakeDamage(float speed, ContactPoint[] contactPoints)
    {
        if (_takenDamage /*|| other.gameObject.layer != LayerMask.NameToLayer("Environment")*/)
        {
            return;
        }
        SpawnDustParticles(contactPoints);
        
        
        StartCoroutine(IFrameCooldown());
        _currentHealth -= speed;
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        UpdateCondition();
    }

    private void SpawnDustParticles(ContactPoint[] contactPoints)
    {
        foreach (var contactPoint in contactPoints)
        {
            var dust = Instantiate(DustParticle, contactPoint.point, Quaternion.identity);
            Destroy(dust, 1f);
        }
    }

    private IEnumerator IFrameCooldown()
    {
        _takenDamage = true;
        yield return new WaitForSeconds(0.5f);
        _takenDamage = false;
    }
    
    private void UpdateCondition()
    {
        foreach(var condition in PackageConditions)
        {
            if (_currentHealth <= condition.Health)
            {
                _boxRenderer.material = condition.Material;
                continue;
            }
            return;
        }
    }
    
    public void PickUp()
    {
        Destroy(_rigidBody);
    }

    public void DropOff()
    {
        _rigidBody = gameObject.AddComponent<Rigidbody>();
        _rigidBody.useGravity = true;
        _rigidBody.isKinematic = false;
    }

    public float GetHealthPercentage()
    {
        return _currentHealth / MaxHP;
    }
}
