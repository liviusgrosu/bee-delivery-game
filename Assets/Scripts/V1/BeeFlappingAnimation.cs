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
        float angle = Mathf.Sin(Time.time * flapSpeed) * flapAngle;

        leftWing.localRotation = leftWingStartRot * Quaternion.Euler(angle, 0, 0);
        rightWing.localRotation = rightWingStartRot * Quaternion.Euler(-angle, 0, 0);
    }
}
