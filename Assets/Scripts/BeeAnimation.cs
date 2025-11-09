using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BeeAnimation : MonoBehaviour
{
    public Transform FreeLookingPole;
    public Transform FreeLookingTarget;
    public static BeeAnimation Instance;
    private Animator _animator;
    [SerializeField] 
    private MultiAimConstraint multiAimConstraint;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _animator = GetComponent<Animator>();
        PlayerFlyingMovement.StunnedEvent += Stun;
    }

    void Update()
    {
        FreeLookingTarget.position = PlayerFlyingMovement.Instance.IsFreeLooking 
            ? FreeLookingPole.position
            : transform.position;
    }

    private void Stun(bool state)
    {
        if (!state)
        {
            return;
        }
        
        _animator.SetTrigger("Stun");
    }
    
    public void PickUp()
    {
        _animator.SetTrigger("Pick Up");
        multiAimConstraint.weight = 0f;
    }

    public void DropOff()
    {
        _animator.SetTrigger("Drop Off");
        multiAimConstraint.weight = 1f;
    }
}
