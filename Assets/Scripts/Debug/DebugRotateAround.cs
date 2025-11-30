using System.Collections;
using UnityEngine;

public class DebugRotateAround : MonoBehaviour
{
    [SerializeField] private float animationTime = 7.5f;
    [SerializeField] private float rotationAmount = 90f;
    private Quaternion _startRotation, _targetRotation;
    private bool _triggered;
    
    private void Start()
    {
        _startRotation = transform.rotation;
        _targetRotation = transform.rotation * Quaternion.Euler(0, -rotationAmount, 0);
    }

    private void Update()
    {
        if (!_triggered && Input.GetKeyDown(KeyCode.Alpha2))
        {
            _triggered = true;
            StartCoroutine(Rotate());
        }
    }

    private IEnumerator Rotate()
    {
        var timePassed = 0f;
        while (timePassed < animationTime)
        {
            var step = timePassed / animationTime;
            transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, step);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }
}
