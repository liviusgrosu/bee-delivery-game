using UnityEngine;

public class BeeFace : MonoBehaviour
{
    public GameObject IdleFace;
    public GameObject DazedFace;

    private void Awake()
    {
        PlayerFlyingMovement.StunnedEvent += ToggleDazedFace;
    }
    
    void ToggleDazedFace(bool state)
    {
        IdleFace.SetActive(!state);
        DazedFace.SetActive(state);
    }
}
