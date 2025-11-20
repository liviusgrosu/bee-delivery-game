using System.Collections;
using UnityEngine;

public class Rubble : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private Collider _collider;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Pickup(Vector3 playerPosition, Transform parent)
    {
        Destroy(_rigidBody);
        var distance = (transform.position - playerPosition).magnitude;
        StartCoroutine(PickUpPackageAnimation(transform.position, parent, Vector3.up * distance));
        transform.parent = parent;
    }

    private IEnumerator PickUpPackageAnimation(Vector3 start, Transform parent, Vector3 offset)
    {
        _collider.enabled = false;
        var timePassed = 0f;
        var animationTime = 0.2f;
        while (timePassed < animationTime)
        {
            var step = timePassed / animationTime;
            transform.position = Vector3.Lerp(start, parent.position - offset, step);
            timePassed += Time.deltaTime;
            yield return null;
        }
        transform.parent = parent;
        _collider.enabled = true;
    }

    public void DropOff()
    {
        _rigidBody = gameObject.AddComponent<Rigidbody>();
        _rigidBody.useGravity = true;
        _rigidBody.isKinematic = false;
        transform.parent = null;
    }
}
