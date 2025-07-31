using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BeeAnimation : MonoBehaviour
{
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
        
        _animator =  GetComponent<Animator>();
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
