using UnityEngine;

public class BeeFlappingAnimation : MonoBehaviour
{
    [Header("Wing References")]
    public Transform leftWing;
    public Transform rightWing;

    [Header("Flap Settings")]
    public float flapSpeed = 30f;
    public float sprintFlapSpeed = 50f;
    public float flapAngle = 30f;

    private Quaternion leftWingStartRot;
    private Quaternion rightWingStartRot;

    void Start()
    {
        leftWingStartRot = leftWing.localRotation;
        rightWingStartRot = rightWing.localRotation;
    }

    void Update()
    {
        var speed = PlayerMovement.Instance.IsSprinting ? sprintFlapSpeed : flapSpeed;
        float angle = Mathf.Sin(Time.time * speed) * flapAngle;

        leftWing.localRotation = leftWingStartRot * Quaternion.Euler(angle, 0, 0);
        rightWing.localRotation = rightWingStartRot * Quaternion.Euler(-angle, 0, 0); // mirror the motion
    }
}
