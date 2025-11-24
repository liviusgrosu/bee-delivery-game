using System.Collections;
using UnityEngine;

public class GiantDoorTrigger : MonoBehaviour, IToggleObjects
{
    [SerializeField]
    private Transform endPositionTransform;
    private Vector3 _startPos, _endPos;
    [SerializeField]
    private bool _isOn;
    [SerializeField]
    private ParticleSystem _dustParticles;

    [SerializeField] private float _gateSpeed = 2f;

    private void Awake()
    {
        _startPos = transform.position;
        _endPos = endPositionTransform.position;
    }
    
    public void TriggerOn()
    {
        StopAllCoroutines();
        StartCoroutine(MoveTowards(_endPos));
    }

    public void TriggerOff()
    {
        StopAllCoroutines();
        StartCoroutine(MoveTowards(_startPos));
    }

    private IEnumerator MoveTowards(Vector3 target)
    {
        _dustParticles.Play();
        var beginningPos = transform.position;
        var distance = (target - beginningPos).magnitude;
        var distanceCovered = (target - beginningPos).magnitude;
        var timeCovered = distanceCovered / distance;
        
        var step = 0f;
        var remainingTime = _gateSpeed * timeCovered;
        
        while (step < remainingTime)
        {
            step += Time.deltaTime;
            var progress = step / remainingTime;
            transform.position = Vector3.Lerp(beginningPos, target, progress);
            yield return null;
        }

        transform.position = target;
        _dustParticles.Stop();
    }
}
