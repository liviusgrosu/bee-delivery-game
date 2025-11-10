using UnityEngine;

public class BeeHeadTrack : MonoBehaviour
{
    public Camera FreeLookCamera;
    
    void Update()
    {
        transform.position =  FreeLookCamera.transform.position + FreeLookCamera.transform.forward * 10f;
    }
}
