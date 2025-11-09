using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float frequency = 1f;

    private Vector3 _startPos;

    public void OnEnable()
    {
        _startPos = transform.localPosition;
    }
    
    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = _startPos + new Vector3(0, yOffset, 0);
    }
}
