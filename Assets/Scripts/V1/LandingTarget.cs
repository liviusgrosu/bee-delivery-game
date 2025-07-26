using System.Net.NetworkInformation;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LandingTarget : MonoBehaviour
{
    public Transform PlayerTransform;
    public LayerMask AvoidLayerMask;
    public float minDistanceToShow = 5f;
    private Quaternion _startingRotation;
    private Material _material;
    
    private void Start()
    {
        _startingRotation = transform.rotation;
        _material = GetComponent<Renderer>().material;
        _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, 0f);
    }
    
    private void Update()
    {
        transform.rotation = _startingRotation;
        
        if (Physics.Raycast(PlayerTransform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, ~AvoidLayerMask))
        {
            var distance = Vector3.Distance(PlayerTransform.position, hit.point);
            
            if (distance > minDistanceToShow)
            {
                _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, 0f);
                return;
            }
            
            var alpha = (minDistanceToShow - distance) / minDistanceToShow ;
            transform.position = hit.point + new Vector3(0, 0.1f, 0);
            SetAlpha(alpha);
        }
    }
    
    public void SetAlpha(float a)
    {
        Color color = _material.color;
        color.a = Mathf.Clamp01(a);
        _material.color = color;
    }
}
