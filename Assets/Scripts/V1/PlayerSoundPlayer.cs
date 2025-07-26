using System.Collections;
using UnityEngine;

public class PlayerSoundPlayer : MonoBehaviour
{
    public static PlayerSoundPlayer Instance { get; private set; }
    
    [Header("Audio Clips")]
    public AudioClip backgroundLoopClip;
    public AudioClip flyingLoopClip;
    public AudioClip hurtClip;
    public AudioClip packagePickUp;
    public AudioClip packageDelievered;

    [Header("Pitch Settings")]
    public float normalPitch = 1f;
    public float sprintPitch = 1.5f;

    [Header("Sprint Control")]
    public bool isSprinting = false;

    private AudioSource backgroundSource;
    private AudioSource flyingSource;
    private AudioSource oneShotSource;

    private bool _isHurtCooldown;
    private float _hurtSoundCooldown = 1f;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Create 3 audio sources
        backgroundSource = gameObject.AddComponent<AudioSource>();
        flyingSource = gameObject.AddComponent<AudioSource>();
        oneShotSource = gameObject.AddComponent<AudioSource>();

        // Set up background loop
        backgroundSource.clip = backgroundLoopClip;
        backgroundSource.loop = true;
        backgroundSource.playOnAwake = true;
        backgroundSource.volume = 1f;
        backgroundSource.Play();

        // Set up flying loop
        flyingSource.clip = flyingLoopClip;
        flyingSource.loop = true;
        flyingSource.playOnAwake = true;
        flyingSource.volume = 0.3f;
        flyingSource.pitch = normalPitch;
        flyingSource.Play();

        oneShotSource.volume = 0.4f;
        oneShotSource.playOnAwake = false;
    }

    void Update()
    {
        flyingSource.pitch = PlayerMovement.Instance.IsSprinting ? sprintPitch : normalPitch;
    }

    public void PlayHurtSound()
    {
        if (!_isHurtCooldown)
        {
            _isHurtCooldown = true;
            StartCoroutine(StartHurtCooldown());
            oneShotSource.PlayOneShot(hurtClip);
        }
    }

    public void PackagePickUpSound()
    {
        oneShotSource.PlayOneShot(packagePickUp);
    }

    public void PackageDeliveredSound()
    {
        oneShotSource.PlayOneShot(packageDelievered);
    }
    
    private IEnumerator StartHurtCooldown()
    {
        yield return new WaitForSeconds(_hurtSoundCooldown);
        _isHurtCooldown = false;
    }
}
