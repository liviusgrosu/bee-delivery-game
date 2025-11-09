using UnityEngine;

public class BeeParticleSystem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem;
    
    void Awake()
    {
        PlayerFlyingMovement.StunnedEvent += FireStunEffect;
    }
    
    private void FireStunEffect(bool state)
    {
        if (!state)
        {
            return;
        }
        particleSystem.Play();
    }
}
