using UnityEngine;

public class SlidingBoxTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform box;
    [Tooltip("How much tolerance the box can be within the box")]
    [SerializeField] private float fitmentTolerance = 0.1f;
    
    [SerializeField]
    private Material onMaterial, offMaterial;
    private MeshRenderer _meshRenderer;
    private bool _isOn;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    
    void Update()
    {
        var distance = Vector3.Distance(box.position, transform.position);
        if (distance <= fitmentTolerance && !_isOn)
        {
            TriggerOn();
        }
        else if (distance > fitmentTolerance && _isOn)
        {
            TriggerOff();
        }
    }

    void TriggerOn()
    {
        _meshRenderer.material = onMaterial;
        _isOn = true;
    }

    void TriggerOff()
    {
        _meshRenderer.material = offMaterial;
        _isOn = false;
    }
}
