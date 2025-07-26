using UnityEngine;

public class Spin : MonoBehaviour
{
    public bool SpinX, SpinY, SpinZ;
    public float SpinSpeed = 5f;
    
    void Update()
    {
        var SpinAxis = Vector3.zero;
        if (SpinX)
        {
            SpinAxis += Vector3.right;
        }
        if (SpinY)
        {
            SpinAxis += Vector3.up;
        }
        if (SpinZ)
        {
            SpinAxis += Vector3.forward;
        }

        if (SpinAxis == Vector3.zero)
        {
            return;
        }
        
        transform.Rotate(SpinAxis, SpinSpeed * Time.deltaTime);
    }
}
