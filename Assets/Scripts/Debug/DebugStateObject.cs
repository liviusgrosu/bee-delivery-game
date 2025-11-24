using System;
using UnityEngine;

public class DebugStateObject : MonoBehaviour, IToggleObjects
{
    [SerializeField]
    private Material onMaterial, offMaterial;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = offMaterial;
    }

    public void TriggerOn()
    {
        _meshRenderer.material = onMaterial;
    }

    public void TriggerOff()
    {
        _meshRenderer.material = offMaterial;
    }
}
